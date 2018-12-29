using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voltmeter.UI
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton(_ =>
            {
                var configuration = _.GetService<IConfiguration>();
                var settings = new VoltmeterSettings();
                configuration.Bind(settings);
                return settings;
            });

            RegisterAdapters(services);
            RegisterUseCases(services);

            services.AddHostedService<HostedServices.StatusRefreshing>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void RegisterUseCases(IServiceCollection services)
        {
            services.AddTransient<RefreshEnvironmentStatusUseCase>();
        }

        private static void RegisterAdapters(IServiceCollection services)
        {
            services.AddSingleton<IEnvironmentStatusStore, EnvironmentStatusStore>();
            services.AddSingleton<IEnvironmentDiscovery, EnvironmentDiscovery>();

            services.AddTransient<IEnvironmentStatusProvider, EnvironmentStatusProvider>();
            services.AddTransient<IServiceDiscovery, ServiceDiscovery>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, VoltmeterSettings settings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{environmentName=" + settings.DefaultEnvironmentName + "}");
            });
        }
    }
}
