using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;
using System.Security.Claims;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    [Authorize]
    public class EditPortfolioModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditPortfolioModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Project> Projects { get; set; } = new();

        public void OnGet()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                Projects = new List<Project>();
                return;
            }

            Projects = _context.Projects.Include(p => p.AppUser).Where(p => p.AppUser.Email == email).OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt).ToList();
        }
    }
}
