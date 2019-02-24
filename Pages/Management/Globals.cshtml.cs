using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestNexus.JintInterop;

namespace RestNexus.Pages.Management
{
    public class GlobalsModel : PageModel
    {
        public string Globals { get; set; }

        public void OnGet()
        {
            Globals = JavaScriptEnvironment.Instance.Globals;
        }

        public IActionResult OnPost(IFormCollection formData)
        {
            if (!ModelState.IsValid)
                return Page();

            string newGlobals = formData["Globals"];

            Globals = newGlobals;
            JavaScriptEnvironment.Instance.UpdateGlobals(newGlobals);

            return Page();
        }
    }
}
