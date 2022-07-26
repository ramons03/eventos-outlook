using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure;
using eventosoutlook.Graph;
using Microsoft.OpenApi.Models;

namespace eventosoutlook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Retrieve required permissions from appsettings
            string[] initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eventos", Version = "v1" });
            });
            services
                // Add support for OpenId authentication
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)

                // Microsoft identity platform web app that requires an auth code flow
                .AddMicrosoftIdentityWebApp(Configuration)

                // Add ability to call Microsoft Graph APIs with specific permissions
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)

                // Enable dependency injection for GraphServiceClient
                .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))

                // Add in-memory token cache
                .AddInMemoryTokenCaches();

            // Require an authenticated user
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services
                // Add Razor Pages support
                .AddRazorPages()

                // Add Microsoft Identity UI pages that provide user 
                .AddMicrosoftIdentityUI();
            services.AddScoped<GraphProfileClient>();
            services.AddScoped<GraphCalendarClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "eventos v1"));
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
