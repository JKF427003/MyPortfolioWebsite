using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;

namespace MyPortfolioWebsite.Pages.Portfolio
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public AppUser? UserProfile { get; set; }
        public List<Project> Projects { get; set; } = new();
        public List<string> DisplaySkills { get; set; } = new();

        public void OnGet(string slug)
        {
            UserProfile = _context.AppUsers
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Images)
                .FirstOrDefault(u => u.PublicSlug == slug && u.IsPortfolioPublic);

            if (UserProfile == null)
            {
                return;
            }

            Projects = UserProfile.Projects
                .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .ToList();

            DisplaySkills = !string.IsNullOrWhiteSpace(UserProfile.Skills)
                ? UserProfile.Skills
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList()
                : Projects
                    .SelectMany(p => (p.ProgrammingLanguages ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .Where(language => !string.IsNullOrWhiteSpace(language))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(language => language)
                    .ToList();
        }
    }
}