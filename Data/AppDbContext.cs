using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MyPortfolioWebsite.Models;

namespace MyPortfolioWebsite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
    }
}
