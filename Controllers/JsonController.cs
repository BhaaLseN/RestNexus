using Microsoft.AspNetCore.Mvc;

namespace RestNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonController : ControllerBase
    {
        [HttpGet("{*url}")]
        public ActionResult<object> Get(string url)
        {
            return NotFound();
        }

        [HttpPost("{*url}")]
        public ActionResult<object> Post(string url, [FromBody] object body)
        {
            return NotFound();
        }

        [HttpPut("{*url}")]
        public ActionResult<object> Put(string url, [FromBody] object body)
        {
            return NotFound();
        }
    }
}
