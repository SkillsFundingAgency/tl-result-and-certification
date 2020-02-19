using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Api.Client.Clients;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Application.Configuration;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using Sfa.Tl.ResultsAndCertification.Web.Authentication;
using Sfa.Tl.ResultsAndCertification.Web.Authentication.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.Filters;
using Sfa.Tl.ResultsAndCertification.Web.Loader;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using System;

namespace Sfa.Tl.ResultsAndCertification.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;
        private readonly IWebHostEnvironment _env;

        protected ResultsAndCertificationConfiguration ResultsAndCertificationConfiguration;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ResultsAndCertificationConfiguration = ConfigurationLoader.Load(
               _config[Constants.EnvironmentNameConfigKey],
               _config[Constants.ConfigurationStorageConnectionStringConfigKey],
               _config[Constants.VersionConfigKey],
               _config[Constants.ServiceNameConfigKey]);

            services.AddApplicationInsightsTelemetry();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton(ResultsAndCertificationConfiguration);
            services.AddHttpClient<ITokenRefresher, TokenRefresher>();
            services.AddTransient<CustomCookieAuthenticationEvents>().AddHttpContextAccessor();
            services.AddTransient<ITokenServiceClient, TokenServiceClient>();
            services.AddHttpClient<IResultsAndCertificationInternalApiClient, ResultsAndCertificationInternalApiClient>();
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                config.Filters.Add<CustomExceptionFilterAttribute>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddWebAuthentication(ResultsAndCertificationConfiguration, _env);
            services.AddAuthorization(options =>
            {
                options.AddPolicy(RolesExtensions.RequireTLevelsReviewerAccess, policy => policy.RequireRole(RolesExtensions.SiteAdministrator, RolesExtensions.TlevelsReviewer));
            });

            RegisterDependencies(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithRedirects("/Error/{0}");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<ITlevelLoader, TlevelLoader>();
        }
    }
}
