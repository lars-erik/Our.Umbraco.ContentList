using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.PropertyEditors;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Composition
{
    public static class ContentListRegistration
    {
        public static void AddContentList(this IUmbracoBuilder builder)
        {
            builder.AddContentListDataSources();
            builder.AddContentListEndpoints();
            builder.Services.AddContentListServices();

            builder.PropertyValueConverters().Append<QueryConverter>();
        }

        public static void AddContentListEndpoints(this IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ServerVariablesParsingNotification, AddControllersToApiPaths>();
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
                {
                    options.AddFilter(new UmbracoPipelineFilter(nameof(ContentListController))
                    {
                        Endpoints = app => app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                "ContentList",
                                "umbraco/ourcontentlist/contentlist/{action}",
                                new {Controller = "ContentList"});
                        })
                    });
                }
            );
        }

        public static void AddContentListDataSources(this IUmbracoBuilder builder)
        {
            var allMeta = builder.TypeLoader.GetTypes<IDataSourceMetadata>();
            builder
                .WithListableDataSources()
                .Add(allMeta);
        }

        public static void AddContentListServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(DataSourceMetadataFactory));
            services.AddTransient(typeof(ContentListQueryHandler));
        }
    }

    public class ContentListComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddContentList();
        }
    }

    public class InterceptedEmbeddedFileProvider : IFileProvider
    {
        EmbeddedFileProvider inner;
        public InterceptedEmbeddedFileProvider(Assembly assembly)
        {
            inner = new EmbeddedFileProvider(assembly);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = inner.GetFileInfo(subpath);
            return fileInfo;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var directoryContents = inner.GetDirectoryContents(subpath);
            return directoryContents;
        }

        public IChangeToken Watch(string pattern)
        {
            var changeToken = inner.Watch(pattern);
            return changeToken;
        }
    }

    public class AddControllersToApiPaths : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly LinkGenerator linkGenerator;

        public AddControllersToApiPaths(LinkGenerator linkGenerator)
        {
            this.linkGenerator = linkGenerator;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            var umbracoUrls = ((Dictionary<string, object>)notification.ServerVariables["umbracoUrls"]);

            umbracoUrls.Add(
                "Our.Umbraco.ContentList.Controllers.ContentListApi",
                linkGenerator.GetUmbracoApiServiceBaseUrl<ContentListApiController>(c => c.GetDataSources())
            );
        }
    }
}