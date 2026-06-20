using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;
using System.Security.Claims;
using System.Text.Json;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Project> Projects { get; set; } = new();
        public string LanguageLabelsJson { get; set; } = "[]";
        public string LanguageCountsJson { get; set; } = "[]";
        public int TotalProjects { get; set; }
        public string LatestProjectTitle { get; set; } = "No projects yet";
        public bool HasVerifiedAlternateEmail { get; set; }

        public void OnGet()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email)) { return; }

            Projects = _context.Projects.Include(p => p.AppUser).Where(p => p.AppUser.Email == email).OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt).ToList();

            var languageCounts = Projects.SelectMany(p => (p.ProgrammingLanguages ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).Where(language => !string.IsNullOrWhiteSpace(language)).GroupBy(language => language).OrderByDescending(group => group.Count()).ToDictionary(group => group.Key, group => group.Count());

            LanguageLabelsJson = JsonSerializer.Serialize(languageCounts.Keys);
            LanguageCountsJson = JsonSerializer.Serialize(languageCounts.Values);

            TotalProjects = Projects.Count;
            LatestProjectTitle = Projects.FirstOrDefault()?.Title ?? "No projects yet";

            var user = _context.AppUsers.FirstOrDefault(u => u.Email == email);
            HasVerifiedAlternateEmail = user?.IsAlternateEmailVerified == true;
        }
    }
}
