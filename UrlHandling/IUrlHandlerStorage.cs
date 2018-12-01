using System.Collections.Generic;

namespace RestNexus.UrlHandling
{
    public interface IUrlHandlerStorage
    {
        IEnumerable<UrlHandler> LoadHandlers();
    }
}
