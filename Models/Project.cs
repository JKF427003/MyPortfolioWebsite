namespace MyPortfolioWebsite.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string ShortSummary { get; set; } = "";
        public string Description { get; set; } = "";
        public string? ProgrammingLanguages { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LiveDemoUrl { get; set; }

        public string? CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = default!;

        public List<ProjectImage> Images { get; set; } = new();
    }
}
