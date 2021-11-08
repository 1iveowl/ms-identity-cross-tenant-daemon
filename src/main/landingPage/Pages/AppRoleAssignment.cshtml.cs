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
    [AuthorizeForScopes(Scopes = new[] { "User.Read", "AppRoleAssignment.ReadWrite.All", "Application.Read.All" })]
    public class AppRoleAssignmentModel : PageModel
    {
        const string GraphResourceId = "00000003-0000-0000-c000-000000000000";

        private readonly GraphServiceClient _graphServiceClient;
        private readonly IOptions<AzureAdOptions> _azureAdOptions;
        private readonly IOptions<DaemonPermissionOptions> _daemonPermissionOptions;
        private readonly ILogger<IndexModel> _logger;

        public User UserDetails { get; private set; }

        public AppRoleAssignmentModel(
            GraphServiceClient graphServiceClient,
            IOptions<AzureAdOptions> azureAdOptions,
            IOptions<DaemonPermissionOptions> daemonPermissionOptions,
            ILogger<IndexModel> logger)
        {
            _graphServiceClient = graphServiceClient;
            _azureAdOptions = azureAdOptions;
            _daemonPermissionOptions = daemonPermissionOptions;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            UserDetails = await _graphServiceClient.Me.Request().GetAsync();

            var appServicePrincipal = await _graphServiceClient
                .ServicePrincipals
                .Request()
                .Filter($"appId eq '{_azureAdOptions.Value.ClientId}'")
                .GetAsync();

            var graphServicePrincipal = await _graphServiceClient
                .ServicePrincipals
                .Request()
                .Filter($"appId eq '{GraphResourceId}'")
                .GetAsync();

            var appServicePrincipalId = appServicePrincipal.FirstOrDefault()?.Id;

            var graphServicePrincipalId = graphServicePrincipal.FirstOrDefault()?.Id;

            var appRoleId = graphServicePrincipal
                .FirstOrDefault()?
                .AppRoles
                .Where(appRole => appRole.Value == _daemonPermissionOptions.Value.Scopes).FirstOrDefault()?
                .Id;

            if (appServicePrincipalId is not null
                && graphServicePrincipalId is not null
                && appRoleId is not null)
            {
                var appRoleAssignment = new AppRoleAssignment
                {
                    PrincipalId = Guid.Parse(appServicePrincipalId),
                    ResourceId = Guid.Parse(graphServicePrincipalId),
                    AppRoleId = appRoleId
                };

                var result = await _graphServiceClient.ServicePrincipals[graphServicePrincipalId].AppRoleAssignments
                    .Request()
                    .AddAsync(appRoleAssignment);
            }

            return Page();
        }
    }
}
