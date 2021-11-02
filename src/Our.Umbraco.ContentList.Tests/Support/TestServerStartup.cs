using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Moq;
using Our.Umbraco.ContentList.Controllers;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class TestServerStartup : IStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(typeof(IListableContent).Assembly));
                })
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/Views/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/TestViews/{0}.cshtml");
                })
                .AddApplicationPart(typeof(Web.Program).Assembly)
                .AddApplicationPart(typeof(TestServerStartup).Assembly)
                ;
            
            services.AddTransient<ViewRenderer>();
            services.AddTransient<ContentListQueryHandler>();
            services.AddSingleton(Mock.Of<IHttpContextAccessor>());
            
            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
        }
    }
}
