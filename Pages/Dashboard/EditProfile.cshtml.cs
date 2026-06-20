using Microsoft.AspNetCore.Authorization;
using MyPortfolioWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using MyPortfolioWebsite.Data;
using MyPortfolioWebsite.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;

        public EditProfileModel(AppDbContext context, IWebHostEnvironment env, IEmailSender emailSender)
        {
            _context = context;
            _env = env;
            _emailSender = emailSender;
        }

        [BindProperty] public AppUser Profile { get; set; } = default!;
        [BindProperty] public IFormFile? ProfilePictureFile { get; set; }
        [BindProperty] public string? CroppedPhotoData { get; set; }
        public string? PublicPortfolioUrl { get; set; }

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
                    AccountCreated = DateTime.UtcNow,
                    IsEmailVerified = true,
                    EmailVerifiedAt = DateTime.UtcNow,
                };
                _context.AppUsers.Add(user);
                _context.SaveChanges();
            }

            Profile = user;
            SetPublicPortfolioUrl(Profile.PublicSlug);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToPage("/Account/Login");

            var userInDb = _context.AppUsers.FirstOrDefault(u => u.Email == email);
            if (userInDb == null)
                return RedirectToPage("/Dashboard/EditProfile");

            Profile.PublicSlug = NormalizeSlug(Profile.PublicSlug);
            ValidatePublicSlug(userInDb.Id);

            if (!ModelState.IsValid)
            {
                PopulateDisplayFields(userInDb);
                SetPublicPortfolioUrl(Profile.PublicSlug);
                return Page();
            }

            userInDb.FirstName = Profile.FirstName;
            userInDb.LastName = Profile.LastName;

            var submittedAlternateEmail = Profile.AlternateEmail?.Trim();

            if (!string.Equals(submittedAlternateEmail, userInDb.AlternateEmail, StringComparison.OrdinalIgnoreCase))
            {
                userInDb.PendingAlternateEmail = submittedAlternateEmail;
                userInDb.IsAlternateEmailVerified = false;
                userInDb.AlternateEmailVerificationToken = Guid.NewGuid().ToString("N");
                userInDb.AlternateEmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24);

                if (!string.IsNullOrWhiteSpace(submittedAlternateEmail))
                {
                    var verifyUrl = Url.Page(
                        "/Account/VerifyAlternateEmail",
                        pageHandler: null,
                        values: new { token = userInDb.AlternateEmailVerificationToken },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        submittedAlternateEmail,
                        "Verify your alternate email",
                        $"Click this link to verify your alternate email: <a href=\"{verifyUrl}\">{verifyUrl}</a>");
                }
            }

            userInDb.DateOfBirth = Profile.DateOfBirth;
            userInDb.Gender = Profile.Gender;
            userInDb.Description = Profile.Description;
            userInDb.JobTitle = Profile.JobTitle;
            userInDb.Location = Profile.Location;
            userInDb.GitHubUrl = Profile.GitHubUrl;
            userInDb.LinkedInUrl = Profile.LinkedInUrl;
            userInDb.Skills = Profile.Skills;
            userInDb.PublicSlug = Profile.PublicSlug;
            userInDb.PortfolioHeadline = Profile.PortfolioHeadline;
            userInDb.IsPortfolioPublic = Profile.IsPortfolioPublic;

            if (!string.IsNullOrWhiteSpace(CroppedPhotoData))
            {
                const string prefix = "data:image/jpeg;base64,";

                if (!CroppedPhotoData.StartsWith(prefix, StringComparison.Ordinal))
                {
                    ModelState.AddModelError("ProfilePictureFile", "Invalid cropped image data.");
                    PopulateDisplayFields(userInDb);
                    SetPublicPortfolioUrl(Profile.PublicSlug);
                    return Page();
                }

                var base64 = CroppedPhotoData[prefix.Length..];
                var imageBytes = Convert.FromBase64String(base64);

                const long maxBytes = 2 * 1024 * 1024;

                if (imageBytes.Length > maxBytes)
                {
                    ModelState.AddModelError("ProfilePictureFile", "File is too large after cropping. Max size is 2 MB.");
                    PopulateDisplayFields(userInDb);
                    SetPublicPortfolioUrl(Profile.PublicSlug);
                    return Page();
                }

                var folder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}.jpg";
                var fullPath = Path.Combine(folder, fileName);

                System.IO.File.WriteAllBytes(fullPath, imageBytes);

                DeleteLocalIfOwned(userInDb.ProfilePictureUrl);
                userInDb.ProfilePictureUrl = $"/uploads/profiles/{fileName}";
            }

            _context.SaveChanges();
            return RedirectToPage();
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

        private static string? NormalizeSlug(string? slug)
        {
            return string.IsNullOrWhiteSpace(slug)
                ? null
                : slug.Trim().ToLowerInvariant();
        }

        private void ValidatePublicSlug(int currentUserId)
        {
            if (Profile.IsPortfolioPublic && string.IsNullOrWhiteSpace(Profile.PublicSlug))
            {
                ModelState.AddModelError("Profile.PublicSlug", "A public link name is required when your portfolio is public.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Profile.PublicSlug))
            {
                return;
            }

            if (!Regex.IsMatch(Profile.PublicSlug, "^[a-z0-9]+(?:-[a-z0-9]+)*$"))
            {
                ModelState.AddModelError("Profile.PublicSlug", "Use lowercase letters, numbers, and single hyphens only.");
                return;
            }

            var slugExists = _context.AppUsers.Any(u => u.Id != currentUserId && u.PublicSlug == Profile.PublicSlug);
            if (slugExists)
            {
                ModelState.AddModelError("Profile.PublicSlug", "This public link name is already taken.");
            }
        }

        private void SetPublicPortfolioUrl(string? slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                PublicPortfolioUrl = null;
                return;
            }

            PublicPortfolioUrl = Url.Page(
                "/Portfolio/Index",
                pageHandler: null,
                values: new { slug },
                protocol: Request.Scheme);
        }

        private void PopulateDisplayFields(AppUser user)
        {
            Profile.Id = user.Id;
            Profile.Email = user.Email;
            Profile.ProfilePictureUrl = user.ProfilePictureUrl;
            Profile.AccountCreated = user.AccountCreated;
            Profile.IsEmailVerified = user.IsEmailVerified;
            Profile.IsAlternateEmailVerified = user.IsAlternateEmailVerified;
            Profile.PendingAlternateEmail = user.PendingAlternateEmail;
        }
    }
}
