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
            object result = handler?.Handle(HttpVerb.Get, url, null);
            if (result == null)
                return NotFound();

            return result;
        }

        [HttpPost("{*url}")]
        public ActionResult<object> Post(string url, [FromBody] object body)
        {
            var handler = _urlRepository.Find(url);
            object result = handler?.Handle(HttpVerb.Post, url, body);
            if (result == null)
                return NotFound();

            return result;
        }

        [HttpPut("{*url}")]
        public ActionResult<object> Put(string url, [FromBody] object body)
        {
            var handler = _urlRepository.Find(url);
            object result = handler?.Handle(HttpVerb.Put, url, body);
            if (result == null)
                return NotFound();

            return result;
        }
    }
}
