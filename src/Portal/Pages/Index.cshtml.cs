using Microsoft.AspNetCore.Mvc.RazorPages;
using ImpactHub.ERP.Portal.Context;

namespace ImpactHub.ERP.Portal.Pages;

public class IndexModel : PageModel
{
    private readonly ICurrentPortalContext _context;

    public IndexModel(ICurrentPortalContext context)
    {
        _context = context;
    }

    public string Email => _context.CurrentUser.Email;

    public void OnGet()
    {
    }
}

