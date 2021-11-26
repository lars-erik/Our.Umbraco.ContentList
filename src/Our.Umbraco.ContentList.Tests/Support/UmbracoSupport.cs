using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;
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
        private readonly Action<IUmbracoBuilder> configureUmbraco;
        private IPublishedSnapshot publishedSnapshot;
        private IPublishedSnapshotAccessor publishedSnapshotAccessor;
        private FakeDataTypeService dataTypeService;
        private FakeContentTypeService contentTypeService;
        private ContentStore contentStore;
        private TestVariationContextAccessor variationContextAccessor;

        public UmbracoSupport(
            BPlusTree<int, ContentNodeKit> localDb = null, 
            Action<IDataTypeService, IContentTypeService> setupContentTypes = null,
            Action<IServiceCollection> configureServices = null,
            Action<IUmbracoBuilder> configureUmbraco = null
            )
        {
            this.localDb = localDb;
            this.setupContentTypes = setupContentTypes;
            this.configureServices = configureServices;
            this.configureUmbraco = configureUmbraco;
        }

        public new IServiceProvider Services => base.Services;

        public T GetService<T>()
        {
            return GetRequiredService<T>();
        }

        public override void Setup()
        {
            base.Setup();
            SetupHttpContext();

            StaticServiceProvider.Instance = Services;

            if (setupContentTypes != null)
            {
                setupContentTypes(GetService<IDataTypeService>(), GetService<IContentTypeService>());
                SetupContentCache();
            }
        }

        protected override void CustomTestSetup(IUmbracoBuilder builder)
        {
            configureUmbraco?.Invoke(builder);
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

        private void SetupHttpContext()
        {
            ReplaceHttpRequest("", new Dictionary<string, StringValues>());
        }

        public IUmbracoContext GetUmbracoContext()
        {
            var ctxFact = GetRequiredService<IUmbracoContextFactory>();
            var ctxRef = ctxFact.EnsureUmbracoContext();

            var accessor = GetRequiredService<IUmbracoContextAccessor>();

            var ctx = ctxRef.UmbracoContext;
            return ctx;
        }


        public void TearDownUmbraco()
        {
            base.TearDown_Logging();
            Task.Run(async () => await base.TearDownAsync()).Wait();
        }

        public void ReplaceHttpRequest(string path, Dictionary<string, StringValues> queryValues)
        {
            var httpContext = CreateHttpContext("example.com", path, queryValues);
            Mock.Get(GetService<IHttpContextAccessor>())
                .Setup(x => x.HttpContext)
                .Returns(httpContext);
        }

        private DefaultHttpContext CreateHttpContext(string host, string path, Dictionary<string, StringValues> queryValues)
        {
            var ctx = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString(host),
                    Path = new PathString(path),
                    Query = new QueryCollection(queryValues)
                },
                RequestServices = Services
            };

            return ctx;
        }
    }
}
