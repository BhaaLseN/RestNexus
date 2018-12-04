using Microsoft.AspNetCore.Mvc.RazorPages;
using RestNexus.Models;
using RestNexus.UrlHandling;

namespace RestNexus.Pages.Management
{
    public class DetailModel : PageModel
    {
        private readonly UrlRepository _urlRepository;
        public DetailModel(UrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }
        public void OnGet(string urlTemplate)
        {
            UrlHandlerViewModel handlerViewModel;

            var handler = _urlRepository.Get(urlTemplate);
            if (handler != null)
            {
                handlerViewModel = new UrlHandlerViewModel(handler);
            }
            else
            {
                // assume creation if there is no matching handler yet.
                handlerViewModel = new UrlHandlerViewModel { UrlTemplate = urlTemplate };
                IsNew = true;
            }

            Handler = handlerViewModel;
        }
        public UrlHandlerViewModel Handler { get; set; }
        public bool IsNew { get; set; }
    }
}
