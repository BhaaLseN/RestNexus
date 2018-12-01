using System.Linq;

namespace RestNexus.UrlHandling
{
    public abstract class UrlHandler
    {
        public string UrlTemplate { get; set; }

        public abstract object Handle(HttpVerb method, string url, object body);

        // parameters are in the form ":name"
        public static bool IsParameter(string segment) => segment?.FirstOrDefault() == ':';
    }

    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        // the other verbs probably don't make much sense as triggers.
    }
}
