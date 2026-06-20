using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebsite.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MyPortfolioWebsite.Pages.Dashboard.Projects
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int ProjectId { get; set; }

        [BindProperty]
        public string ProjectTitle { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Type the project name to confirm deletion.")]
        public string ConfirmationTitle { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var project = _context.Projects
                .Include(p => p.AppUser)
                .FirstOrDefault(p => p.Id == id && p.AppUser.Email == email);

            if (project == null)
            {
                return RedirectToPage("/Dashboard/EditPortfolio");
            }

            ProjectId = project.Id;
            ProjectTitle = project.Title;

            return Page();
        }

        public IActionResult OnPost()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var project = _context.Projects
                .Include(p => p.AppUser)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == ProjectId && p.AppUser.Email == email);

            if (project == null)
            {
                return RedirectToPage("/Dashboard/EditPortfolio");
            }

            if (!string.Equals(ConfirmationTitle, project.Title, StringComparison.Ordinal))
            {
                ProjectTitle = project.Title;
                ModelState.AddModelError(nameof(ConfirmationTitle), "The project name does not match.");
                return Page();
            }

            DeleteLocalIfOwned(project.CoverImageUrl);

            foreach (var image in project.Images)
            {
                DeleteLocalIfOwned(image.ImageUrl);
            }

            _context.Projects.Remove(project);
            _context.SaveChanges();

            return RedirectToPage("/Dashboard/EditPortfolio");
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