using Microsoft.AspNetCore.Mvc;
using RestNexus.UrlHandling;

namespace RestNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonController : ControllerBase
    {
        private readonly UrlRepository _urlRepository;
        public JsonController(UrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        [HttpGet("{*url}")]
        public ActionResult<object> Get(string url)
        {
            var handler = _urlRepository.Find(url);
            if (handler == null)
                return NotFound();

            return handler.Handle(url, null);
        }

        [HttpPost("{*url}")]
        public ActionResult<object> Post(string url, [FromBody] object body)
        {
            var handler = _urlRepository.Find(url);
            if (handler == null)
                return NotFound();

            return handler.Handle(url, body);
        }

        [HttpPut("{*url}")]
        public ActionResult<object> Put(string url, [FromBody] object body)
        {
            var handler = _urlRepository.Find(url);
            if (handler == null)
                return NotFound();

            return handler.Handle(url, body);
        }
    }
}
