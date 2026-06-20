using MyPortfolioWebsite.Models;
using Xunit;

namespace MyPortfolioWebsite.Tests
{
    public class ProjectModelTests
    {
        [Fact]
        public void Project_DefaultDates_AreSet()
        {
            var project = new Project();

            Assert.True(project.CreatedAt <= DateTime.UtcNow);
            Assert.Null(project.UpdatedAt);
        }

        [Fact]
        public void AppUser_Projects_DefaultsToEmptyList()
        {
            var user = new AppUser();

            Assert.NotNull(user.Projects);
            Assert.Empty(user.Projects);
        }
    }
}