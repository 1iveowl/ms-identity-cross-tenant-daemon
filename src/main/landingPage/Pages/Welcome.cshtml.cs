using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Options;

namespace LandingPage.Pages
{
    [Authorize]
    [AuthorizeForScopes(Scopes = new[] { "User.Read" })]
    public class WelcomeModel : PageModel
    {
        private readonly IOptions<AzureAdOptions> _azureAdOptions;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string? TenantId { get; private set; } = "<Something went wrong.>";

        public WelcomeModel(
            IOptions<AzureAdOptions> azureAdOptions, 
            ILogger<IndexModel> logger)
        {
            _azureAdOptions = azureAdOptions;
            _logger = logger;
        }


        public async Task<IActionResult> OnGet()
        {
            TenantId = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            return Page();
        }

        public IActionResult OnPostAppRoleAssignment()
            => RedirectToPage($"/{nameof(AppRoleAssignmentModel).Replace("Model", string.Empty)}");
    }
}
