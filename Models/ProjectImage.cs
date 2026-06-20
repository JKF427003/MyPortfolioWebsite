namespace MyPortfolioWebsite.Models
{
    public class ProjectImage
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; } = "";
        public bool IsCoverImage { get; set; }
        public DateTime UploadedAt  { get; set; } = DateTime.UtcNow;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
