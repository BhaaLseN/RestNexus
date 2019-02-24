using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestNexus.Models;
using RestNexus.UrlHandling;

namespace RestNexus.Pages.Management
{
    public class ListModel : PageModel
    {
        private readonly UrlRepository _urlRepository;
        public ListModel(UrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }
        public void OnGet()
        {
            var handlers = _urlRepository.GetAll();

            Handlers = handlers.Select(h => new UrlHandlerViewModel(h)).ToArray();
        }
        public IActionResult OnDelete(string urlTemplate)
        {
            if (_urlRepository.Remove(urlTemplate))
                return new OkResult();

            return new NotFoundResult();
        }
        public IEnumerable<UrlHandlerViewModel> Handlers { get; set; }
    }
}
