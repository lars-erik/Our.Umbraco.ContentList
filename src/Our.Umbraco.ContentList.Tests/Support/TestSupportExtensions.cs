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

            // This is required by RazorViewEngine
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddSingleton(diagnosticSource);

            // Get the MvcBuilder and add web and package view providers
            services.AddMvc()
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new FakeFileSystemProvider("Our.Umbraco.ContentList"));
                    options.FileProviders.Add(new FakeFileSystemProvider("Our.Umbraco.ContentList.Web"));
                })
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/Views/TestViews/{0}.cshtml");
                })
                // Required for runtime compilation of views from those assemblies (the slow part?)
                .AddApplicationPart(typeof(IListableContent).Assembly)
                .AddApplicationPart(typeof(Web.Program).Assembly)
                ;

            // Required to discover view components (same as .AddViewComponentsAsServices())
            services.Replace(ServiceDescriptor.Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

        }
    }
}
