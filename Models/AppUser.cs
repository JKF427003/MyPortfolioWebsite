namespace MyPortfolioWebsite.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        // Required
        public string Email { get; set; } = default!;

        // Optional (nullable)
        public string? GoogleId { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AlternateEmail { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Description { get; set; }

        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;

        public List<Project> Projects { get; set; } = new();
    }
}
