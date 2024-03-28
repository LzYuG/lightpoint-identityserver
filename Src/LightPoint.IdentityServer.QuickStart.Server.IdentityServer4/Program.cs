using IdentityServer4;
using LightPoint.IdentityServer.Blazor;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.HttpOverrides;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.Stores;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.IdentityServer4Imps;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.ResourceServices;
using LightPoint.IdentityServer.QuickStart.Foundation.LightPointIdentityServer;
using IdentityServer4.Extensions;
using Serilog.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLightPointIdentityService<ServiceImpFactoryImp>(builder.Configuration, builder.Environment)
    .WithAccountPageService<IdentityServerAccountPageService>()
    .WithConsentPageService<IdentityServerConsentPageService>()
    .WithDevicePageService<IdentityServerDevicePageService>()
    .WithIdentityServerResourceSecretService<IdentityServerResourceSecretService>();

// cookie policy to deal with temporary browser incompatibilities
builder.Services.AddSameSiteCookiePolicy();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;

    options.EmitScopesAsSpaceDelimitedStringInJwt = true;

    options.MutualTls.Enabled = true;
    options.MutualTls.DomainName = "mtls";
    //options.MutualTls.AlwaysEmitConfirmationClaim = true;
})
    .AddClientStore<LPClientStore>()
    .AddResourceStore<LPResourceStore>()
    .AddDeviceFlowStore<LPDeviceFlowStore>()
    .AddPersistedGrantStore<LPPersistedGrantStore>()
    .AddSigningCredential()
    .AddExtensionGrantValidator<ExtensionGrantValidator>()
    .AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
    .AddJwtBearerClientAuthentication()
    .AddAppAuthRedirectUriValidator()
    .AddProfileService<HostProfileService>()
    .AddCustomTokenRequestValidator<ParameterizedScopeTokenRequestValidator>()
    .AddScopeParser<ParameterizedScopeParser>()
    .AddMutualTlsSecretValidators();


//builder.Services.AddExternalIdentityProviders();

builder.Services.AddAuthentication()
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.RevocationMode = X509RevocationMode.NoCheck;
    });

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
           .MinimumLevel.Override("System", LogEventLevel.Warning)
           .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
           .Enrich.FromLogContext()
           .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
           .CreateLogger();

builder.Host.UseSerilog();

//builder.Services.AddCertificateForwardingForNginx();

//builder.Services.AddLocalApiAuthentication(principal =>
//{
//    principal.Identities.First().AddClaim(new Claim("additional_claim", "additional_value"));

//    return Task.FromResult(principal);
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseDeveloperExceptionPage();

app.UseStaticFiles();

app.UseCors(options => options
               .SetIsOriginAllowed(x => true)
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

app.Use(async (ctx, next) =>
{
    if (builder.Environment.IsProduction())
    {
        if(ctx.Connection.LocalPort.ToString() != "80" && ctx.Connection.LocalPort.ToString() != "443")
        {
            ctx.SetIdentityServerOrigin("https://" + ctx.Request.Host.ToString() + ":9001");
        }
    }
    await next();
});

app.UseRouting();
// сеохв╒╡А
app.UseMiddleware<TenantMiddleware>();

app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapRazorPages();

app.Run();



public static class ServiceExtensions
{
    public static IServiceCollection AddExternalIdentityProviders(this IServiceCollection services)
    {
        // configures the OpenIdConnect handlers to persist the state parameter into the server-side IDistributedCache.
        services.AddOidcStateDataFormatterCache("aad", "demoidsrv");

        services.AddAuthentication()
            .AddOpenIdConnect("Google", "Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

                options.Authority = "https://accounts.google.com/";
                options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";

                options.CallbackPath = "/signin-google";
                options.Scope.Add("email");
            })
            .AddOpenIdConnect("demoidsrv", "IdentityServer", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.Authority = "https://demo.identityserver.io/";
                options.ClientId = "login";
                options.ResponseType = "id_token";
                options.SaveTokens = true;
                options.CallbackPath = "/signin-idsrv";
                options.SignedOutCallbackPath = "/signout-callback-idsrv";
                options.RemoteSignOutPath = "/signout-idsrv";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            })
            .AddOpenIdConnect("aad", "Azure AD", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.Authority = "https://login.windows.net/4ca9cb4c-5e5f-4be9-b700-c532992a3705";
                options.ClientId = "96e3c53e-01cb-4244-b658-a42164cb67a9";
                options.ResponseType = "id_token";
                options.CallbackPath = "/signin-aad";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            })
            .AddOpenIdConnect("adfs", "ADFS", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.Authority = "https://adfs.leastprivilege.vm/adfs";
                options.ClientId = "c0ea8d99-f1e7-43b0-a100-7dee3f2e5c3c";
                options.ResponseType = "id_token";

                options.CallbackPath = "/signin-adfs";
                options.SignedOutCallbackPath = "/signout-callback-adfs";
                options.RemoteSignOutPath = "/signout-adfs";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        return services;
    }

    public static void AddCertificateForwardingForNginx(this IServiceCollection services)
    {
        services.AddCertificateForwarding(options =>
        {
            options.CertificateHeader = "X-SSL-CERT";

            options.HeaderConverter = (headerValue) =>
            {
                X509Certificate2 clientCertificate = null!;

                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(Uri.UnescapeDataString(headerValue));
                    clientCertificate = new X509Certificate2(bytes);
                }

                return clientCertificate;
            };
        });
    }
}