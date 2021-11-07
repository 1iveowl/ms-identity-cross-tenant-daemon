using Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.UI;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Using the Options pattern for settings: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<AzureAdOptions>(configuration.GetSection(AzureAdOptions.AzureAdAppRegistration));
builder.Services.Configure<MSGraphOptions>(configuration.GetSection(MSGraphOptions.MSGraphSettings));

string[]? initialScopes = configuration.GetValue<string>($"{MSGraphOptions.MSGraphSettings}:Scopes")?.Split(' ');

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(configuration.GetSection(AzureAdOptions.AzureAdAppRegistration))
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddMicrosoftGraph(configuration.GetSection(MSGraphOptions.MSGraphSettings))
                .AddInMemoryTokenCaches();

//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//          .AddMicrosoftIdentityWebApp(configuration, AzureAdOptions.AzureAdAppRegistration)
//.EnableTokenAcquisitionToCallDownstreamApi(new string[] { "user.read" })
//.AddInMemoryTokenCaches();

// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/2-WebApp-graph-user/2-1-Call-MSGraph
//builder.Services
//    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(options =>
//    {
//        configuration.Bind(AzureAdOptions.AzureAdAppRegistration, options);
//        options.Events.OnTokenValidated = async context =>
//        {
//            var tenantId = context.SecurityToken.Claims.FirstOrDefault(x => x.Type == "tid" || x.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

//            if (string.IsNullOrWhiteSpace(tenantId))
//                throw new UnauthorizedAccessException("Unable to get tenantId from token.");
//        };

//        options.Events.OnAuthenticationFailed = (context) =>
//        {
//            if (context.Exception != null && context.Exception is UnauthorizedAccessException)
//            {
//                context.Response.Redirect("/Home/UnauthorizedTenant");
//                context.HandleResponse(); // Suppress the exception
//            }

//            return Task.FromResult(0);
//        };

//    });
//.AddMicrosoftIdentityWebAppAuthentication(configuration, AzureAdOptions.AzureAdAppRegistration)
//.EnableTokenAcquisitionToCallDownstreamApi(
//options =>
//{
//    configuration.Bind(AzureAdOptions.AzureAdAppRegistration, options);
//},
//new[] { "AppRoleAssignment.ReadWrite.All" })
//.EnableTokenAcquisitionToCallDownstreamApi(new[] { "https://graph.microsoft.com/.default" })
//.AddMicrosoftGraph(configuration.GetSection(MSGraphOptions.MSGraphSettings))
//.AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;

});

// Add services to the container.
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
