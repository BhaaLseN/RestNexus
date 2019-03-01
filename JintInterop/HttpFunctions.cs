using System;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestNexus.JintInterop
{
    public class HttpFunctions
    {
        private static readonly HttpClient _client = new HttpClient();

        private static void AddBody(HttpRequestMessage request, object body)
        {
            if (body == null)
                return;

            var content = new StringContent(JsonConvert.SerializeObject(body));
            content.Headers.ContentType.MediaType = "application/json";
            request.Content = content;
        }

        private static void AddHeaders(HttpRequestMessage request, object headers)
        {
            if (headers == null)
                return;

            if (headers is ExpandoObject expando)
            {
                foreach (var foo in expando)
                    request.Headers.Add(foo.Key, foo.Value?.ToString());
                return;
            }

            var jHeaders = JObject.Parse(JsonConvert.SerializeObject(headers));
            foreach (var header in jHeaders.Properties())
                request.Headers.Add(header.Name, header.Value.Value<string>());
        }

        private static HttpResponse PerformRequest(HttpMethod method, string url, object body = null, object headers = null)
        {
            var result = new HttpResponse();
            try
            {
                var request = new HttpRequestMessage(method, url);
                AddHeaders(request, headers);
                AddBody(request, body);

                var response = _client.SendAsync(request).GetAwaiter().GetResult();
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
        public HttpResponse get(string url) => get(url, null);
        public HttpResponse get(string url, object headers) => PerformRequest(HttpMethod.Get, url, headers: headers);
        public HttpResponse post(string url, object body) => post(url, body, null);
        public HttpResponse post(string url, object body, object headers) => PerformRequest(HttpMethod.Post, url, body: body, headers: headers);
        public HttpResponse put(string url, object body) => put(url, body, null);
        public HttpResponse put(string url, object body, object headers) => PerformRequest(HttpMethod.Put, url, body: body, headers: headers);

        public HttpResponse request(string url, string method) => request(url, method, null, null);
        public HttpResponse request(string url, string method, object body) => request(url, method, body, null);
        public HttpResponse request(string url, string method, object body, object headers) => PerformRequest(new HttpMethod(method), url, body: body, headers: headers);
    }
    public class HttpResponse
    {
        public string response { get; set; }
        public int statusCode { get; set; }
    }
}
