using System;
using RestNexus.UrlHandling;

namespace RestNexus.Models
{
    public class UrlHandlerViewModel
    {
        public UrlHandlerViewModel()
        {
            HandlerType = UrlHandlerType.JavaScript;
        }
        public UrlHandlerViewModel(UrlHandler model)
            : this()
        {
            if (!(model is JavaScriptUrlHandler jsHandler))
                throw new NotSupportedException("Only JavaScript handlers are supported at this point.");

            UrlTemplate = jsHandler.UrlTemplate;
            Content = jsHandler.Script;
        }

        public string UrlTemplate { get; set; }
        public UrlHandlerType HandlerType { get; set; }
        public string Content { get; set; }
    }
    public enum UrlHandlerType
    {
        JavaScript = 0,
    }
}
