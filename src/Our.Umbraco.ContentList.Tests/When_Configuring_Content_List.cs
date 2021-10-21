using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Composition;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Integration.Implementations;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    [UseReporter(typeof(VisualStudioReporter))]
    public class When_Configuring_Content_List
    {
        private UmbracoSupport support;
        private ContentListApiController controller;

        [SetUp]
        public void Setup()
        {
            support = new UmbracoSupport();

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
            var hostingEnvironment = Mock.Of<IHostingEnvironment>();
            services.AddSingleton(typeof(IHostingEnvironment), hostingEnvironment);
            Mock.Get(hostingEnvironment).Setup(x => x.MapPathWebRoot(It.IsAny<string>())).Returns("/");

            builder.Build();

            var provider = services.BuildServiceProvider();

            controller = provider.GetService<ContentListApiController>();
        }

        [TearDown]
        public void TearDown()
        {
            support.TearDownUmbraco();
        }

        [Test]
        public void Shows_Registered_DataSources()
        {
            var sourceTypes = controller.GetDataSources();
            
            Approvals.VerifyAll("Source types", sourceTypes, x => $"{x.Name,-30}{x.Key}");
        }

        [Test]
        public void Shows_Sample_Template_When_None_Exist()
        {
            var sourceTypes = controller.GetTemplates();
            
            Approvals.VerifyAll("Templates", sourceTypes, x => $"{x.Name,-30}{x.DisplayName}");
        }
    }
}
