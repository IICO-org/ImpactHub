using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace ImpactHub.ERP.WebHost.Pages
{
    [AllowAnonymous] // Only this page bypasses auth
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Already logged in? → go to portal
            if (User.Identity?.IsAuthenticated == true)
                return Redirect("/portal");

            return Page();
        }

        public IActionResult OnPost()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = "/portal"
                },
                OpenIdConnectDefaults.AuthenticationScheme
            );
        }
    }
}
