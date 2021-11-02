using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Options;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false);

var configuration = builder.Build();

var tenantId = configuration
    .GetSection(AzureAdOptions.AzureAdAppRegistration)
    .GetSection(nameof(AzureAdOptions.TenantId))
    .Value;

var clientId = configuration
    .GetSection(AzureAdOptions.AzureAdAppRegistration)
    .GetSection(nameof(AzureAdOptions.ClientId))
    .Value;

var secret = configuration
    .GetSection(AzureAdOptions.AzureAdAppRegistration)
    .GetSection(nameof(AzureAdOptions.ClientSecret))
    .Value;

var authority = configuration
    .GetSection(AzureAdOptions.AzureAdAppRegistration)
    .GetSection(nameof(AzureAdOptions.Authority))
    .Value;

var app = ConfidentialClientApplicationBuilder.Create(clientId)
    .WithClientSecret(secret)
    .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
    .WithTenantId(tenantId)
    .Build();

var scopes = new string[] { $"https://graph.microsoft.com/.default", };

var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");