using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestNexus.JintInterop;
using RestNexus.UrlHandling;

namespace RestNexus
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var contentRootProvider = _env.ContentRootFileProvider;
            services.AddSingleton(contentRootProvider);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton(Configuration);

            services.AddSingleton<UrlRepository>();
            services.AddSingleton<IUrlHandlerStorage, XmlUrlHandlerStorage>();
            services.AddSingleton<JavaScriptEnvironment>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=management}/{action=index}/{id?}");
            });

            JavaScriptEnvironment.Instance = app.ApplicationServices.GetService<JavaScriptEnvironment>();
        }
    }
}
