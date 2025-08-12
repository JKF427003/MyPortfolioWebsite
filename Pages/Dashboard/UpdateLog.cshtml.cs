using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyPortfolioWebsite.Services;

namespace MyPortfolioWebsite.Pages.Dashboard
{
    public class UpdateLogModel : PageModel
    {
        private readonly IVersionService _versionService;

        public string CurrentVersion { get; private set; }
        public List<string> Changelog { get; private set; }

        public UpdateLogModel(IVersionService versionService)
        {
            _versionService = versionService;
        }

        public void OnGet()
        {
            CurrentVersion = _versionService.GetCurrentVersion();
            Changelog = _versionService.GetChangelog();
        }
    }
}
