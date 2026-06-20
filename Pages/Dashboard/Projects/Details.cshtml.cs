using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;
using System.Security.Claims;

namespace MyPortfolioWebsite.Pages.Dashboard.Projects
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Project Project { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var project = _context.Projects
                .Include(p => p.AppUser)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id && p.AppUser.Email == email);

            if (project == null)
            {
                return RedirectToPage("/Dashboard/EditPortfolio");
            }

            Project = project;
            return Page();
        }
    }
}