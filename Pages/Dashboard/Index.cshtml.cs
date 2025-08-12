using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Layout"] = "_DashboardLayout";
        }
    }
}
