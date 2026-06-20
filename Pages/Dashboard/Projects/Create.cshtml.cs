using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;
using System.ComponentModel;
using System.Security.Claims;

namespace MyPortfolioWebsite.Pages.Dashboard.Projects
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = new();
        [BindProperty]
        public IFormFile? CoverImageFile { get; set; }
        [BindProperty]
        public List<IFormFile> GalleryImageFiles { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = _context.AppUsers.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return RedirectToPage("/Dashboard/EditProfile");
            }

            if (CoverImageFile != null && CoverImageFile.Length > 0)
            {
                Project.CoverImageUrl = SaveProjectImage(CoverImageFile);
            }

            foreach (var imageFile in GalleryImageFiles)
            {
                if (imageFile.Length == 0)
                {
                    continue;
                }

                Project.Images.Add(new ProjectImage
                {
                    ImageUrl = SaveProjectImage(imageFile),
                    IsCoverImage = false,
                    UploadedAt = DateTime.UtcNow
                });
            }

            Project.AppUserId = user.Id;
            Project.CreatedAt = DateTime.UtcNow;
            Project.UpdatedAt = null;

            _context.Projects.Add(Project);
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
    }
}
