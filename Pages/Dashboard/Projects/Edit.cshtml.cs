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
        [BindProperty]
        public List<IFormFile> GalleryImageFiles { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var project = _context.Projects.Include(p => p.AppUser).Include(p => p.Images).FirstOrDefault(p => p.Id == id && p.AppUser.Email == email);

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

            var projectInDb = _context.Projects.Include(p => p.AppUser).Include(p => p.Images).FirstOrDefault(p => p.Id == Project.Id && p.AppUser.Email == email);

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
                var coverImageUrl = SaveProjectImage(CoverImageFile, nameof(CoverImageFile));

                if (coverImageUrl == null)
                {
                    Project = projectInDb;
                    return Page();
                }

                DeleteLocalIfOwned(projectInDb.CoverImageUrl);
                projectInDb.CoverImageUrl = coverImageUrl;
            }

            foreach (var imageFile in GalleryImageFiles)
            {
                if (imageFile.Length == 0)
                {
                    continue;
                }

                var imageUrl = SaveProjectImage(imageFile, nameof(GalleryImageFiles));

                if (imageUrl == null)
                {
                    Project = projectInDb;
                    return Page();
                }

                projectInDb.Images.Add(new ProjectImage
                {
                    ImageUrl = imageUrl,
                    IsCoverImage = false,
                    UploadedAt = DateTime.UtcNow
                });
            }

            _context.SaveChanges();

            return RedirectToPage(new { id = projectInDb.Id });
        }

        private string? SaveProjectImage(IFormFile imageFile, string modelStateKey)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            const long maxBytes = 4 * 1024 * 1024;

            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError(modelStateKey, "Only JPG, PNG, or WEBP files are allowed.");
                return null;
            }

            if (imageFile.Length > maxBytes)
            {
                ModelState.AddModelError(modelStateKey, "File is too large. Maximum size is 4 MB.");
                return null;
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

        public IActionResult OnPostRemoveGalleryImage(int id, int imageId)
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

            var image = project.Images.FirstOrDefault(i => i.Id == imageId);

            if (image == null)
            {
                return RedirectToPage(new { id });
            }

            DeleteLocalIfOwned(image.ImageUrl);
            _context.ProjectImages.Remove(image);
            _context.SaveChanges();

            return RedirectToPage(new { id });
        }
    }
}
