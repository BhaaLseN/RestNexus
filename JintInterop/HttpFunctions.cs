using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestNexus.JintInterop
{
    public class HttpFunctions
    {
        private static readonly HttpClient _client = new HttpClient();

        private static HttpContent ConvertContent(object body)
        {
            if (body == null)
                return null;

            var content = new StringContent(JsonConvert.SerializeObject(body));
            content.Headers.ContentType.MediaType = "application/json";
            return content;
        }

        private static HttpResponse PerformRequest(Func<HttpClient, Task<HttpResponseMessage>> call)
        {
            var result = new HttpResponse();
            try
            {
                var response = call(_client).GetAwaiter().GetResult();
                result.response = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                result.statusCode = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                result.response = ex.Message;
                result.statusCode = -1;
            }
            return result;
        }

        // disable member name style violations; they are lower-cased to match common JavaScript style.
        // this intentionally spans until the end of the file.
#pragma warning disable IDE1006
        public HttpResponse get(string url) => PerformRequest(client => client.GetAsync(url));
        public HttpResponse post(string url, object body) => PerformRequest(client => client.PostAsync(url, ConvertContent(body)));
        public HttpResponse put(string url, object body) => PerformRequest(client => client.PutAsync(url, ConvertContent(body)));
    }
    public class HttpResponse
    {
        public string response { get; set; }
        public int statusCode { get; set; }
    }
}
