namespace MyPortfolioWebsite.Services
{
    public interface IVersionService
    {
        string GetCurrentVersion();
        List<string> GetChangelog();
    }

    public class VersionService : IVersionService
    {
        public string GetCurrentVersion() => "v0.5.0";

        public List<string> GetChangelog() => new List<string>
        {
            "(23/06/2026) v0.5.0 - Added DevOps challenge setup with GitHub Actions CI, automated tests, artifact publishing, environment configuration, and local deployment documentation",
            "(23/06/2026) v0.4.0 - Added public project details pages, improved public project cards, fixed project image sizing, added language chips, and added visitor-facing project actions",
            "(23/06/2026) v0.3.0 - Added public portfolio link controls, copy public URL button, public slug validation, uniqueness checks, and public/private portfolio behavior",
            "(20/06/2026) v0.2.0 - Added dashboard widgets, project statistics, featured project rotation, language chart, alternate email status, and user-specific project filtering",
            "(20/06/2026) v0.1.0 - Added portfolio project creation, editing, deletion with typed confirmation, project details, cover image upload, and gallery image uploads",
            "(19/06/2026) v0.0.1 - Redesigned Edit Profile, added profile photo upload, crop preview, default/remove photo options, alternate email verification, and public portfolio fields",
            "(07/08/2025) v0.0.0 - Initial setup, Google authentication, dashboard layout, navbar login/logout, profile image rendering from Google"
        };
    }
}
