using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using RestNexus.JintInterop;
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

            Definitions.Add(("environment/globals.js", EnvironmentGlobals()));
        }

        private static readonly Regex JsonPasswordField = new Regex(@"(?<nameQuote>[""'])(?<name>.*?(?:password|token).*?)\k<nameQuote>\s*:\s*(?<valueQuote>[""'])(?<value>.*?)\k<valueQuote>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static string EnvironmentGlobals()
        {
            // try to not put plaintext passwords or tokens in the source
            string globals = JsonPasswordField.Replace(JavaScriptEnvironment.Instance.Globals, "${nameQuote}${name}${nameQuote}: ${valueQuote}secret${valueQuote}");
            return "var globals = " + globals;
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

        public IActionResult OnPost(string urlTemplate, IFormCollection formData)
        {
            if (!ModelState.IsValid)
                return Page();

            Handler = new UrlHandlerViewModel
            {
                UrlTemplate = formData["Handler.UrlTemplate"],
                Content = formData["code"],
            };

            // update urlTemplate in case this is a new one.
            // we need the old path to indicate which one needs to be updated.
            if (string.IsNullOrEmpty(urlTemplate))
                urlTemplate = Handler.UrlTemplate;

            _urlRepository.Update(urlTemplate, Handler.UrlTemplate, Handler.Content);

            return RedirectToPage("List");
        }
    }
}
