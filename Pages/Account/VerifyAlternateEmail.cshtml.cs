using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPortfolioWebsite.Data;

namespace MyPortfolioWebsite.Pages.Account
{
    public class VerifyAlternateEmailModel : PageModel
    {
        private readonly AppDbContext _context;

        public VerifyAlternateEmailModel(AppDbContext context)
        {
            _context = context;
        }

        public string Message { get; set; } = "";

        public void OnGet(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Message = "Invalid verification link.";
                return;
            }

            var user = _context.AppUsers.FirstOrDefault(u => u.AlternateEmailVerificationToken == token);

            if (user == null)
            {
                Message = "Invalid verification link.";
                return;    
            }

            if (user.AlternateEmailVerificationTokenExpires < DateTime.UtcNow)
            {
                Message = "This verification link has expired";
                return;
            }

            user.AlternateEmail = user.PendingAlternateEmail;
            user.PendingAlternateEmail = null;
            user.IsAlternateEmailVerified = true;
            user.AlternateEmailVerifiedAt = DateTime.UtcNow;
            user.AlternateEmailVerificationToken = null;
            user.AlternateEmailVerificationTokenExpires = null;

            _context.SaveChanges();

            Message = "Your alternate email has been verified.";
        }
    }
}
