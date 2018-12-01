namespace RestNexus.UrlHandling
{
    public class JavaScriptUrlHandler : UrlHandler
    {
        public string ScriptFile { get; set; }

        public override object Handle(string url, object body)
        {
            return "";
        }
    }
}
