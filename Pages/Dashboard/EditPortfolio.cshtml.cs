using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    [Authorize]
    public class EditPortfolioModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
