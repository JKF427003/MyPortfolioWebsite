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
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = new();
        [BindProperty]
        public IFormFile? CoverImageFile { get; set; }

        public IActionResult OnGet(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var project = _context.Projects.Include(p => p.AppUser).FirstOrDefault(p => p.Id == id && p.AppUser.Email == email);

            if (project == null)
            {
                return RedirectToPage("/Dashboard/EditPortfolio");
            }

            Project = project;
            return Page();
        }

        public IActionResult OnPost()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var projectInDb = _context.Projects.Include(p => p.AppUser).FirstOrDefault(p => p.Id == Project.Id && p.AppUser.Email == email);

            if (projectInDb == null)
            {
                return RedirectToPage("/Dashboard/EditPortfolio");
            }

            projectInDb.Title = Project.Title;
            projectInDb.ShortSummary = Project.ShortSummary;
            projectInDb.Description = Project.Description;
            projectInDb.ProgrammingLanguages = Project.ProgrammingLanguages;
            projectInDb.GitHubUrl = Project.GitHubUrl;
            projectInDb.LiveDemoUrl = Project.LiveDemoUrl;
            projectInDb.UpdatedAt = DateTime.UtcNow;

            if (CoverImageFile != null && CoverImageFile.Length > 0)
            {
                DeleteLocalIfOwned(projectInDb.CoverImageUrl);
                projectInDb.CoverImageUrl = SaveProjectImage(CoverImageFile);
            }

            _context.SaveChanges();

            return RedirectToPage("/Dashboard/EditPortfolio");
        }

        private string SaveProjectImage(IFormFile imageFile)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            const long maxBytes = 4 * 1024 * 1024;

            if (!allowed.Contains(ext))
            {
                throw new InvalidOperationException("Only JPG, PNG, or WEBP files are allowed.");
            }

            if (imageFile.Length > maxBytes)
            {
                throw new InvalidOperationException("File is too large. Maximum size is 4 MB.");
            }

            var folder = Path.Combine("wwwroot", "uploads", "projects");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using var stream = System.IO.File.Create(fullPath);
            imageFile.CopyTo(stream);

            return $"/uploads/projects/{fileName}";
        }

        private void DeleteLocalIfOwned(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            if (url.StartsWith("/uploads/projects/", StringComparison.Ordinal))
            {
                var physical = Path.Combine("wwwroot", url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(physical))
                {
                    try { System.IO.File.Delete(physical); } catch { }
                }
            }
        }
    }
}
