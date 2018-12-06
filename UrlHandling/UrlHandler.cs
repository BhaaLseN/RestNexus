using System;
using System.Collections.Generic;
using System.Linq;

namespace RestNexus.UrlHandling
{
    public abstract class UrlHandler
    {
        public string UrlTemplate { get; set; }

        public abstract object Handle(UrlRequest request);

        // parameters are in the form ":name"
        public static bool IsParameter(string segment) => segment?.FirstOrDefault() == ':';
        public static string GetParameterName(string segment)
        {
            if (!IsParameter(segment))
                return null;

            return segment.Substring(1);
        }
        public static Dictionary<string, string> ExtractParameters(string urlTemplate) => ExtractParameters(urlTemplate, urlTemplate);
        public static Dictionary<string, string> ExtractParameters(string urlTemplate, string url)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // no template; assume no parameters
            if (string.IsNullOrEmpty(urlTemplate))
                return result;
            // no url; assume we want to parse the template itself (to get parameter keys)
            if (string.IsNullOrEmpty(url))
                url = urlTemplate;

            string[] templateSegments = urlTemplate.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string[] urlSegments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Math.Min(templateSegments.Length, urlSegments.Length); i++)
            {
                // try to extract a parameter name from this segment. will be empty when it isn't a parameter.
                string parameterName = GetParameterName(templateSegments[i]);
                if (string.IsNullOrEmpty(parameterName))
                    continue;

                string parameterValue = urlSegments[i];
                if (IsParameter(parameterValue))
                    parameterValue = null;
                result[parameterName] = parameterValue;
            }

            return result;
        }
    }

    public class UrlRequest
    {
        public UrlRequest(HttpVerb method, string url, IReadOnlyDictionary<string, string> headers, object body)
        {
            Method = method;
            Url = url;
            Headers = headers;
            Body = body;
        }

        public HttpVerb Method { get; }
        public string Url { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public object Body { get; }
    }

    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        // the other verbs probably don't make much sense as triggers.
    }
}
