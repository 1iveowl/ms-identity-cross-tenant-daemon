using Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.UI;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Using the Options pattern for settings: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<AzureAdOptions>(configuration.GetSection(AzureAdOptions.AzureAdAppRegistration));
builder.Services.Configure<MSGraphOptions>(configuration.GetSection(MSGraphOptions.MSGraphSettings));


// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/2-WebApp-graph-user/2-1-Call-MSGraph
builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        configuration.Bind(AzureAdOptions.AzureAdAppRegistration, options);
        options.Events.OnTokenValidated = async context =>
        {
            var tenantId = context.SecurityToken.Claims.FirstOrDefault(x => x.Type == "tid" || x.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new UnauthorizedAccessException("Unable to get tenantId from token.");
        };

        options.Events.OnAuthenticationFailed = (context) =>
        {
            if (context.Exception != null && context.Exception is UnauthorizedAccessException)
            {
                context.Response.Redirect("/Home/UnauthorizedTenant");
                context.HandleResponse(); // Suppress the exception
            }

            return Task.FromResult(0);
        };

    })
    //.AddMicrosoftIdentityWebAppAuthentication(configuration, AzureAdOptions.AzureAdAppRegistration)
    .EnableTokenAcquisitionToCallDownstreamApi(
        options =>
        {
            configuration.Bind(AzureAdOptions.AzureAdAppRegistration, options);
        },
        new[] { "AppRoleAssignment.ReadWrite.All" })
    //.EnableTokenAcquisitionToCallDownstreamApi(new[] { "https://graph.microsoft.com/.default" })
    .AddMicrosoftGraph(configuration.GetSection(MSGraphOptions.MSGraphSettings))
    .AddInMemoryTokenCaches();

//builder.Services.AddControllersWithViews(options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//});

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;

});

// Add services to the container.
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();


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
