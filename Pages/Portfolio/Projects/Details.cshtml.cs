using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;

namespace MyPortfolioWebsite.Pages.Portfolio.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public AppUser? UserProfile { get; set; }
        public Project? Project { get; set; }
        public List<string> ProjectLanguages { get; set; } = new();

        public IActionResult OnGet(string slug, int id)
        {
            UserProfile = _context.AppUsers
                .FirstOrDefault(u => u.PublicSlug == slug && u.IsPortfolioPublic);

            if (UserProfile == null)
            {
                return NotFound();
            }

            Project = _context.Projects
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id && p.AppUserId == UserProfile.Id);

            if (Project == null)
            {
                return NotFound();
            }

            ProjectLanguages = (Project.ProgrammingLanguages ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            return Page();
        }
    }
}
