using System;
using System.Linq;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Tests.Common;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;
using Umbraco.Cms.Web.Common.DependencyInjection;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Tests.Support
{
    [UmbracoTest(Database = UmbracoTestOptions.Database.None)]
    public class UmbracoSupport : UmbracoIntegrationTest
    {
        private readonly BPlusTree<int, ContentNodeKit> localDb;
        private readonly Action<IDataTypeService, IContentTypeService> setupContentTypes;
        private readonly Action<IServiceCollection> configureServices;
        private IPublishedSnapshot publishedSnapshot;
        private IPublishedSnapshotAccessor publishedSnapshotAccessor;
        private FakeDataTypeService dataTypeService;
        private FakeContentTypeService contentTypeService;
        private ContentStore contentStore;
        private TestVariationContextAccessor variationContextAccessor;

        public UmbracoSupport(
            BPlusTree<int, ContentNodeKit> localDb = null, 
            Action<IDataTypeService, IContentTypeService> setupContentTypes = null,
            Action<IServiceCollection> configureServices = null)
        {
            this.localDb = localDb;
            this.setupContentTypes = setupContentTypes;
            this.configureServices = configureServices;
        }

        public new IServiceProvider Services => base.Services;

        public override void Setup()
        {
            base.Setup();
            SetupRequest();

            StaticServiceProvider.Instance = Services;

            if (setupContentTypes != null)
            {
                setupContentTypes(GetService<IDataTypeService>(), GetService<IContentTypeService>());
                SetupContentCache();
            }
        }

        public T GetService<T>()
        {
            return GetRequiredService<T>();
        }

        public void SetupContentCache()
        {
            var publishedModelFactory = GetRequiredService<IPublishedModelFactory>();
            contentStore = new ContentStore(
                publishedSnapshotAccessor,
                variationContextAccessor,
                Mock.Of<ILogger>(),
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()),
                publishedModelFactory, // TODO: Use the real model factory?
                localDb
            );

            var publishedContentTypeFactory = GetRequiredService<IPublishedContentTypeFactory>();
            using (contentStore.GetScopedWriteLock(GetRequiredService<IScopeProvider>()))
            {
                //contentStore.UpdateDataTypesLocked(dataTypeService.GetAll()));
                contentStore.UpdateContentTypesLocked(contentTypeService.GetAll()
                    .Select(x => publishedContentTypeFactory.CreateContentType(x)));
                contentStore.SetAllLocked(localDb.Values);
            }

            var storeSnapshot = contentStore.CreateSnapshot();

            var cache = new ContentCache(
                false,
                storeSnapshot,
                NoAppCache.Instance,
                NoAppCache.Instance,
                Mock.Of<IDomainCache>(),
                new OptionsWrapper<GlobalSettings>(GlobalSettings),
                variationContextAccessor
            );

            Mock.Get(publishedSnapshot).Setup(x => x.Content).Returns(cache);
        }

        // TODO: Customizable URL
        private void SetupRequest()
        {
            var httpContext = GetRequiredService<IHttpContextAccessor>().HttpContext;
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost", 443);
            httpContext.Request.PathBase = new PathString();
            httpContext.Request.Path = new PathString("/");
            httpContext.Request.QueryString = QueryString.Empty;
            var url = httpContext.Request.GetEncodedUrl();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.RemoveAll(x => x.ServiceType == typeof(IDataTypeService));
            dataTypeService = new FakeDataTypeService();
            services.AddSingleton<IDataTypeService>(dataTypeService);

            services.RemoveAll(x => x.ServiceType == typeof(IContentTypeService));
            contentTypeService = new FakeContentTypeService();
            services.AddSingleton<IContentTypeService>(contentTypeService);

            publishedSnapshot = Mock.Of<IPublishedSnapshot>();
            Mock.Get(publishedSnapshot).Setup(x => x.SnapshotCache).Returns(NoAppCache.Instance);
            services.AddSingleton(publishedSnapshot);

            publishedSnapshotAccessor = Mock.Of<IPublishedSnapshotAccessor>();
            Mock.Get(publishedSnapshotAccessor).Setup(x => x.TryGetPublishedSnapshot(out publishedSnapshot)).Returns(true);
            services.AddSingleton(publishedSnapshotAccessor);

            variationContextAccessor = new TestVariationContextAccessor();
            variationContextAccessor.VariationContext = new VariationContext();
                services.AddSingleton<IVariationContextAccessor>(variationContextAccessor);

            var snapshotService = new FakePublishedSnapshotService(publishedSnapshot);
            services.AddSingleton<IPublishedSnapshotService>(snapshotService);

            configureServices?.Invoke(services);
        }

        public IUmbracoContext GetUmbracoContext()
        {
            var ctxFact = GetRequiredService<IUmbracoContextFactory>();
            var ctxRef = ctxFact.EnsureUmbracoContext();
            var ctx = ctxRef.UmbracoContext;
            return ctx;
        }


        public void TearDownUmbraco()
        {
            base.TearDown_Logging();
            Task.Run(async () => await base.TearDownAsync()).Wait();
        }
    }
}
