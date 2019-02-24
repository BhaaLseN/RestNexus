using System.Collections.Generic;

namespace RestNexus.UrlHandling
{
    public interface IUrlHandlerStorage
    {
        IEnumerable<UrlHandler> LoadHandlers();
        void SaveHandler(string urlTemplate, UrlHandler handler);
        bool DeleteHandler(string urlTemplate);
    }
}
