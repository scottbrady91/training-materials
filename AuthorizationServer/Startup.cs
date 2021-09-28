using System.Collections.Generic;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddIdentityServer(options => options.EmitScopesAsSpaceDelimitedStringInJwt = false)
                .AddInMemoryClients(Clients)
                .AddInMemoryApiResources(ApiResources)
                .AddInMemoryApiScopes(ApiScopes)
                .AddInMemoryIdentityResources(new List<IdentityResource>
                {
                    new IdentityResources.OpenId(), new IdentityResources.Profile(), new IdentityResources.Email(), new IdentityResources.Phone()
                })
                .AddTestUsers(TestUsers.Users)
                .AddRedirectUriValidator<NoopRedirectValidator>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        public List<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "machine",
                ClientSecrets = new []{new Secret("secret".Sha256())},
                AllowedScopes = new []{"weather_api", "weather_api.read"},
                AllowedGrantTypes = GrantTypes.ClientCredentials
            },
            new Client
            {
                ClientId = "web",
                ClientSecrets = new []{new Secret("web_secret".Sha256())},
                AllowedScopes = new []{"openid", "profile", "weather_api", "weather_api.read"},
                RedirectUris = new []{"https://localhost:5002/signin-oidc"},
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                AllowPlainTextPkce = false
            }
        };

        public List<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource("weather_api") { Scopes = new[] { "weather_api", "weather_api.read" } }
        };

        public List<ApiScope> ApiScopes = new List<ApiScope>
        {
            new ApiScope("weather_api"),
            new ApiScope("weather_api.read")
        };
    }

    // NEVER USE IN PRODUCTION - DEMO ONLY
    public class NoopRedirectValidator : IRedirectUriValidator
    {
        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client) => Task.FromResult(true);
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client) => Task.FromResult(true);
    }
}
