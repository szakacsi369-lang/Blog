using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace BlogApi.Controllers
{
    [Route("blogger")]
    [ApiController]
    public class BloggerController : ControllerBase
    {
        public  readonly string ConnectionString = "server=localhost;database=blog;user=root;password=";

        [HttpGet("bloggers")]
        public object GetAllBlogger()
        {
            List<Blogger> lista = new List<Blogger>();

            var connector = new MySqlConnection(ConnectionString);
            
            connector.Open();

            string sql = "SELECT * FROM blogger";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };

                lista.Add(blogger);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = lista };
        }

        [HttpGet("byId/{id}")]
        public object GetBloggerById([FromRoute]int id) 
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"Select * FROM blogger Where id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            object result = null;

            if (datareader.Read())
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };

                result = new { message = "Sikeres lekérdezés", result = blogger };
            }

            else 
            {
                result = new { message = "Sikertelen lekérdezés", result = "" };
            }

            connector.Close();

            return result;
        }

        [HttpPost("login")]
        public object PostBloggerLogin([FromBody]LoginBlogerDTO loginBloggerDto) 
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"Select * FROM blogger Where email = @email AND password=@password";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@email", loginBloggerDto.Email);
            cmd.Parameters.AddWithValue("@password", loginBloggerDto.Password);

            var datareader = cmd.ExecuteReader();
            object result = null;

            if (datareader.Read())
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };

                result = new { message = "Reisztrált tag", result = blogger };
            }

            else
            {
                result = new { message = "Nem regisztrált tag", result = "" };
            }

            connector.Close();

            return result;
        }

        [HttpPost("register")]
        public object AddNewBlogger([FromBody] RegisterBloggerDto registerBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogger`(`name`, `email`, `age`, `password`, `RegistrationTime`) VALUES (@name,@email,@age,@password,@registationTime)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", registerBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", registerBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", registerBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", registerBloggerDto.Password);
            cmd.Parameters.AddWithValue("@registrationTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "sikeres hozzáadás", result = registerBloggerDto };
        }

        [HttpPut("update/id")]
        public object UpdateBlogger([FromRoute]int id, [FromBody]UpdateBloggerDTO updateBloggerDTO ) 
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogger` SET `name`=@name,`email`=@email,`age`=@age,`password`=@password, WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", updateBloggerDTO.Name);
            cmd.Parameters.AddWithValue("@email", updateBloggerDTO.Email);
            cmd.Parameters.AddWithValue("@age", updateBloggerDTO.Age);
            cmd.Parameters.AddWithValue("@password", updateBloggerDTO.Password);
            cmd.Parameters.AddWithValue("@id", id);

            object result = null;

            if (cmd.ExecuteNonQuery() > 0)
            {
                result = StatusCode(200, new { message = "Sikeres frissítés", result = updateBloggerDTO });
            }
            else
            {
                result = NotFound(new { message = "Nincs ilyen tag", result = updateBloggerDTO });
            }

            connector.Close();

            return result;
        }

        [HttpDelete("delete")]
        public object DeleteBlogger([FromQuery]int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM blogger WHERE id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            object result = null;

            if (cmd.ExecuteNonQuery() > 0)
            {
                result = StatusCode(204, new { message = "Sikeres törlés"});
            }
            else
            {
                result = NotFound(new { message = "Nincs ilyen tag"});
            }

            connector.Close();

            return result;
        }

    }
}
