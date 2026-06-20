namespace MyPortfolioWebsite.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public bool IsEmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public string? GoogleId { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AlternateEmail { get; set; }
        public bool IsAlternateEmailVerified { get; set; }
        public DateTime? AlternateEmailVerifiedAt { get; set; }
        public string? PendingAlternateEmail { get; set; }
        public string? AlternateEmailVerificationToken { get; set; }
        public DateTime? AlternateEmailVerificationTokenExpires { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Description { get; set; }
        public string? JobTitle { get; set; }
        public string? Location { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Skills { get; set; }
        public string? PublicSlug { get; set; }
        public bool IsPortfolioPublic { get; set; }
        public string? PortfolioHeadline { get; set; }

        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;

        public List<Project> Projects { get; set; } = new();
    }
}
