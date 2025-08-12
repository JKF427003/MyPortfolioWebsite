namespace MyPortfolioWebsite.Services
{
    public interface IVersionService
    {
        string GetCurrentVersion();
        List<string> GetChangelog();
    }

    public class VersionService : IVersionService
    {
        public string GetCurrentVersion() => "v0.0.0";

        public List<string> GetChangelog() => new List<string>
        {
            "(07/08/2025) v0.0.0 - Initial setup, Google authentication, dashboard layout, navbar login/logout, profile image rendering from Google"
        };
    }
}
