namespace RestNexus.UrlHandling
{
    public abstract class UrlHandler
    {
        public string UrlTemplate { get; set; }

        public abstract object Handle(string url, object body);
    }
}
