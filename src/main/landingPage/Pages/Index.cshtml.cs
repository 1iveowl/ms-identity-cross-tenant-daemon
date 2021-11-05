using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Options;
using System.Security.Claims;

namespace LandingPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IOptions<AzureAdOptions> _azureAdOptions;
        private readonly ILogger<IndexModel> _logger;

        public string? AdminConsentLink { get; private set; }

        public IndexModel(
            IOptionsSnapshot<AzureAdOptions> azureAdOptions,
            ILogger<IndexModel> logger)
        {
            _azureAdOptions = azureAdOptions;
            _logger = logger;

        }

        public async Task<IActionResult> OnGet()
        {
            var userTenantId = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            AdminConsentLink = $"https://login.microsoftonline.com/{userTenantId?.ToString()}/v2.0/adminconsent" +
                $"?client_id={_azureAdOptions.Value.ClientId}" +
                $"&scope=https://graph.microsoft.com/TeamsActivity.Read.All" +
                $"&redirect_uri=https://localhost/admin-consent" +
                $"&state=test";

            return Page();

        //https://login.microsoftonline.com/{tenant}/v2.0/adminconsent
        //?client_id = 6731de76 - 14a6 - 49ae - 97bc - 6eba6914391e
        //          & scope = https://graph.microsoft.com/Calendars.Read https://graph.microsoft.com/Mail.Send
        //&redirect_uri = http://localhost/myapp/permissions
        //&state = 12345
        }
    }
}