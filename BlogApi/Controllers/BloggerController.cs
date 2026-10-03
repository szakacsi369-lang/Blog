using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return "Hello world";
        }
    }
}
