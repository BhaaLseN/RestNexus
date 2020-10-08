using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestNexus.JintInterop;
using RestNexus.UrlHandling;

namespace RestNexus
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
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

            services.AddRazorPages();
            services.AddSingleton(Configuration);

            services.AddSingleton<UrlRepository>();
            services.AddSingleton<IUrlHandlerStorage, XmlUrlHandlerStorage>();
            services.AddSingleton<JavaScriptEnvironment>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new
                    {
                        controller = "management",
                    });
            });

            JavaScriptEnvironment.Instance = app.ApplicationServices.GetService<JavaScriptEnvironment>();
        }
    }
}
