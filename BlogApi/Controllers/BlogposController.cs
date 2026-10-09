using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BlogApi.Controllers
{
    [Route("blogpost")]
    [ApiController]
    public class BlogposController : ControllerBase
    {
        public readonly string ConnectionString = "server=localhost;database=blog;user=root;password=";

        [HttpGet("blogposts")]
        public object GetAllBlogPosts()
        {
            List<Blogpost> lista = new List<Blogpost>();

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = "SELECT * FROM blogpost";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var bloggpost = new Blogpost
                {
                    Id = datareader.GetInt32(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2),
                    postTime = datareader.GetDateTime(3),
                    updateTime = datareader.GetDateTime(4),
                    blogId = datareader.GetInt32(5)
                };

                lista.Add(bloggpost);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = lista };
        }

        [HttpGet("byId/{id}")]
        public object GetBloggPostsById([FromRoute] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"Select * FROM blogpost Where id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            object result = null;

            if (datareader.Read())
            {
                var bloggpost = new Blogpost
                {
                    Id = datareader.GetInt32(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2),
                    postTime = datareader.GetDateTime(3),
                    updateTime = datareader.GetDateTime(4),
                    blogId = datareader.GetInt32(5)
                };

                result = new { message = "Sikeres lekérdezés", result = bloggpost };
            }

            else
            {
                result = new { message = "Sikertelen lekérdezés", result = "" };
            }

            connector.Close();

            return result;
        }

        [HttpPost("Title")]
        public object PostBloggpostTitle([FromBody] BlogpostTitleDTO BlogpostTitleDTO)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"Select * FROM blogpost Where title = @title";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", BlogpostTitleDTO.Title);

            var datareader = cmd.ExecuteReader();

            object result = null;

            if (datareader.Read())
            {
                var bloggpost = new Blogpost
                {
                    Id = datareader.GetInt32(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2),
                    postTime = datareader.GetDateTime(3),
                    updateTime = datareader.GetDateTime(4),
                    blogId = datareader.GetInt32(5)
                };

                result = new { message = "Reisztrált tag", result = bloggpost };
            }

            else
            {
                result = new { message = "Nem regisztrált tag", result = "" };
            }

            connector.Close();

            return result;
        }

        [HttpPost("newbloggpost")]
        public object AddNewBlogger([FromBody] NewBloggpostDTO NewBloggpostDTO)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogpost`(`Title`, `Content`, `blogId`, `postTime`, `updateTime` ) VALUES (@title,@content,@blogId,@postTime,@updateTime)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", NewBloggpostDTO.Title);
            cmd.Parameters.AddWithValue("@content", NewBloggpostDTO.Content);
            cmd.Parameters.AddWithValue("@blogId", NewBloggpostDTO.blogId);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "sikeres hozzáadás", result = NewBloggpostDTO };
        }

        [HttpPut("update/post/{id}")]
        public object UpdateBloggpost([FromRoute] int id, [FromBody] updateBloggpostDTO updateBloggpostDTO)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogpost` SET `title`=@title,`content`=@content WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", updateBloggpostDTO.Title);
            cmd.Parameters.AddWithValue("@content", updateBloggpostDTO.Content);
            cmd.Parameters.AddWithValue("@id", id);

            object result = null;

            if (cmd.ExecuteNonQuery() > 0)
            {
                result = StatusCode(200, new { message = "Sikeres frissítés", result = updateBloggpostDTO });
            }
            else
            {
                result = NotFound(new { message = "Nincs ilyen tag", result = updateBloggpostDTO });
            }

            connector.Close();

            return result;
        }


        [HttpDelete("delete")]
        public object DeleteBlogger([FromQuery] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM blogpost WHERE id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            object result = null;

            if (cmd.ExecuteNonQuery() > 0)
            {
                result = StatusCode(204, new { message = "Sikeres törlés" });
            }
            else
            {
                result = NotFound(new { message = "Nincs ilyen tag" });
            }

            connector.Close();

            return result;
        }
    }
}
