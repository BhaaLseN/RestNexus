using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using RestNexus.Models;
using RestNexus.UrlHandling;

namespace RestNexus.Pages.Management
{
    public class DetailModel : PageModel
    {
        private readonly UrlRepository _urlRepository;
        private readonly IFileProvider _fileProvider;

        public List<(string FileName, string Content)> Definitions { get; } = new List<(string FileName, string Content)>();
        public UrlHandlerViewModel Handler { get; set; }
        public bool IsNew { get; set; }

        public DetailModel(UrlRepository urlRepository, IFileProvider fileProvider)
        {
            _urlRepository = urlRepository;
            _fileProvider = fileProvider;

            foreach (var definitionInfo in _fileProvider.GetDirectoryContents("wwwroot/definitions/"))
            {
                if (definitionInfo.IsDirectory)
                    continue;

                string fileName = Path.ChangeExtension(Path.GetFileNameWithoutExtension(definitionInfo.Name), ".js");
                using (var contentStream = definitionInfo.CreateReadStream())
                using (var streamReader = new StreamReader(contentStream))
                    Definitions.Add((fileName, streamReader.ReadToEnd()));
            }
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
    }
}
