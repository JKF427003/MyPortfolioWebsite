using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;
using System.Security.Claims;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditProfileModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty] public AppUser Profile { get; set; } = default!;
        [BindProperty] public IFormFile? ProfilePictureFile { get; set; }

        public IActionResult OnGet()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToPage("/Account/Login");

            var user = _context.AppUsers.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new AppUser
                {
                    Email = email,
                    GoogleId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    FirstName = HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value ?? HttpContext.User.FindFirst("given_name")?.Value,
                    LastName = HttpContext.User.FindFirst(ClaimTypes.Surname)?.Value ?? HttpContext.User.FindFirst("family_name")?.Value,
                    Name = HttpContext.User.Identity?.Name,
                    ProfilePictureUrl = HttpContext.User.FindFirst("urn:google:picture")?.Value,
                    AccountCreated = DateTime.UtcNow
                };
                _context.AppUsers.Add(user);
                _context.SaveChanges();
            }

            Profile = user;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToPage("/Account/Login");

            var userInDb = _context.AppUsers.FirstOrDefault(u => u.Email == email);
            if (userInDb == null)
                return RedirectToPage("/Dashboard/EditProfile");

            userInDb.FirstName = Profile.FirstName;
            userInDb.LastName = Profile.LastName;
            userInDb.AlternateEmail = Profile.AlternateEmail;
            userInDb.DateOfBirth = Profile.DateOfBirth;
            userInDb.Gender = Profile.Gender;
            userInDb.Description = Profile.Description;

            if (ProfilePictureFile != null && ProfilePictureFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(ProfilePictureFile.FileName).ToLowerInvariant();
                const long maxBytes = 2 * 1024 * 1024;

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("ProfilePictureFile", "Only JPG, PNG or WEBP files are allowed.");
                    return Page();
                }

                if (ProfilePictureFile.Length > maxBytes)
                {
                    ModelState.AddModelError("ProfilePictureFile", "File is too large (max 2MB).");
                    return Page();
                }

                var folder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(folder, fileName);
                using var stream = System.IO.File.Create(fullPath);
                ProfilePictureFile.CopyTo(stream);

                DeleteLocalIfOwned(userInDb.ProfilePictureUrl);
                userInDb.ProfilePictureUrl = $"/uploads/profiles/{fileName}";
            }

            _context.SaveChanges();
            return RedirectToPage("/Dashboard/Index");
        }

        public IActionResult OnPostRemovePhoto()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email)) return RedirectToPage("/Account/Login");

            var userInDb = _context.AppUsers.FirstOrDefault(u => u.Email == email);
            if (userInDb == null) return RedirectToPage("/Dashboard/EditProfile");

            DeleteLocalIfOwned(userInDb.ProfilePictureUrl);

            userInDb.ProfilePictureUrl = null;

            _context.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostUseDefaultPhoto()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email)) return RedirectToPage("/Account/Login");

            var userInDb = _context.AppUsers.FirstOrDefault(u => u.Email == email);
            if (userInDb == null) return RedirectToPage("/Dashboard/EditProfile");

            DeleteLocalIfOwned(userInDb.ProfilePictureUrl);

            userInDb.ProfilePictureUrl = "/images/default-profile.png";
            _context.SaveChanges();

            return RedirectToPage();
        }

        private void DeleteLocalIfOwned(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            if (url.StartsWith("/uploads/profiles/", StringComparison.Ordinal))
            {
                var physical = Path.Combine(_env.WebRootPath, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(physical))
                {
                    try { System.IO.File.Delete(physical); } catch { }
                }
            }
        }
    }
}