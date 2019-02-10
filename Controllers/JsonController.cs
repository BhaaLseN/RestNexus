using System;
using System.Linq;
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

        private UrlRequest GetUrlRequest(string url, object body = null)
        {
            if (!Enum.TryParse<HttpVerb>(Request.Method, true, out var method))
                throw new NotSupportedException($"Sorry, Method {Request.Method} is not supported.");
            var headers = Request.Headers.ToDictionary(k => k.Key, v => v.Value.ToString(), StringComparer.OrdinalIgnoreCase);
            return new UrlRequest(method, url, headers, body);
        }

        [HttpGet("{*url}")]
        public ActionResult<object> Get(string url)
        {
            var handler = _urlRepository.Find(url);
            object result = handler?.Handle(GetUrlRequest(url));
            if (result == null)
                return NotFound();

            return result;
        }

        [HttpPost("{*url}")]
        public ActionResult<object> Post(string url, [FromBody] object body)
        {
            var handler = _urlRepository.Find(url);
            object result = handler?.Handle(GetUrlRequest(url, body));
            if (result == null)
                return NotFound();

            return result;
        }

        [HttpPut("{*url}")]
        public ActionResult<object> Put(string url, [FromBody] object body)
        {
            var handler = _urlRepository.Find(url);
            object result = handler?.Handle(GetUrlRequest(url, body));
            if (result == null)
                return NotFound();

            return result;
        }
    }
}
