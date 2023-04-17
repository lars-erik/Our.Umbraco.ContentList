using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using Lucene.Net.Analysis.Cjk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Composition;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Tests.DataSources;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Integration.Implementations;
using VerifyNUnit;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class When_Configuring_Content_List
    {
        private IHostingEnvironment hostingEnvironment;
        private IServiceProvider provider;
        private UmbracoSupport support;

        [SetUp]
        public void Setup()
        {
            support = new UmbracoSupport(configureServices: services =>
            {
                services.AddTransient(typeof(ContentListApiController));

                services.AddRenderingSupport();

                /*
                services.Replace(
                    new ServiceDescriptor(typeof(IHostingEnvironment), hostingEnvironment)
                );
                */
            }, configureUmbraco: builder =>
            {
                new ContentListComposer().Compose(builder);
            });

            support.SetupUmbraco();

            /*
        var typeLoader = new TypeLoader(
                new TypeFinder(Mock.Of<ILogger<TypeFinder>>(), new DefaultUmbracoAssemblyProvider(GetType().Assembly, Mock.Of<ILoggerFactory>(), Enumerable.Empty<string>())),
                Mock.Of<IRuntimeHash>(),
                Mock.Of<IAppPolicyCache>(),
                new DirectoryInfo(@"C:\temp"),
                Mock.Of<ILogger<TypeLoader>>(),
                Mock.Of<IProfiler>()
            );
            var services = new ServiceCollection();

            var builder = new UmbracoBuilder(services, Mock.Of<IConfiguration>(), typeLoader);
            new ContentListComposer().Compose(builder);

            services.AddSingleton(typeof(ILocalizationService), Mock.Of<ILocalizationService>());
            services.AddTransient(typeof(ContentListApiController));
            services.AddSingleton(typeof(IHostingEnvironment), hostingEnvironment);

            services.AddRenderingSupport();

            builder.Build();

            provider = services.BuildServiceProvider();
            */
            provider = support.Services;

            var env = provider.GetService<IHostingEnvironment>();
            //var webEnv = provider.GetService<IWebHostingEnvironment>();
        }

        [TearDown]
        public void TearDown()
        {
            ContentListApiController.ResetMapPath();
        }

        [Test]
        public async Task Shows_Registered_DataSources()
        {
            var controller = provider.GetService<ContentListApiController>();

            var sourceTypes = controller?.GetDataSources();
            
            await Verifier.Verify(sourceTypes.Select(x => $"{x.Name,-46}{x.Key}"));
        }

        [Test]
        public async Task Shows_Sample_Template_When_None_Exist()
        {
            ContentListApiController.MapPath = (env, relativePath) =>
                relativePath switch
                {
                    "~/Views/Partials/ContentList" => AppDomain.CurrentDomain.BaseDirectory,
                    "~/App_Plugins/Our.Umbraco.ContentList/Views/ContentList/ListViews" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Our.Umbraco.ContentList.Web\App_Plugins\Our.Umbraco.ContentList\Views\ContentList\Listviews"),
                    _ => throw new Exception("Path not registered")
                };

            var controller = provider.GetService<ContentListApiController>();

            var sourceTypes = controller?.GetTemplates();

            await Verifier.Verify(sourceTypes.Select(x => $"{x.Name,-30}{x.DisplayName}"));
        }

        [Test]
        public async Task Shows_Templates_From_Partial_Views()
        {
            ContentListApiController.MapPath = (env, relativePath) =>
                relativePath switch
                {
                    "~/Views/Partials/ContentList" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Our.Umbraco.ContentList.Web\Views\Partials\ContentList"),
                    "~/App_Plugins/Our.Umbraco.ContentList/Views/ContentList/ListViews" => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Our.Umbraco.ContentList.Web\App_Plugins\Our.Umbraco.ContentList\Views\ContentList\Listviews"),
                    _ => throw new Exception("Path not registered")
                };

            var controller = provider.GetService<ContentListApiController>()!;

            var sourceTypes = controller.GetTemplates();

            await Verifier.Verify(sourceTypes.Select(x => $"{x.Name,-30} {x.Compiles,-5} {x.DisplayName} ({x.Parameters.Length})"));
        }
    }
}
