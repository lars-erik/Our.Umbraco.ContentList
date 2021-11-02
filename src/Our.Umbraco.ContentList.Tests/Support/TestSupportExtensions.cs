using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public static class TestSupportExtensions
    {
        public static void AddRenderingSupport(this IServiceCollection services)
        {
            services.AddTransient<ViewRenderer>();

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddSingleton(diagnosticSource);

            services.AddMvc()
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new FakeFileSystemProvider("Our.Umbraco.ContentList.Web"));
                    options.FileProviders.Add(new EmbeddedFileProvider(typeof(IListableContent).Assembly));
                })
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/Views/TestViews/{0}.cshtml");
                })
                .AddApplicationPart(typeof(IListableContent).Assembly)
                .AddApplicationPart(typeof(Web.Program).Assembly);

            services.Replace(ServiceDescriptor.Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

        }
    }
}
