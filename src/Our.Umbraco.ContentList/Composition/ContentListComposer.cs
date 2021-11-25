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
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Composition
{
    public class ContentListComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var allMeta = builder.TypeLoader.GetTypes<IDataSourceMetadata>();
            builder
                .WithListableDataSources()
                .Add(allMeta);

            builder.Services.AddTransient(typeof(DataSourceMetadataFactory));
            builder.Services.AddTransient(typeof(ContentListQueryHandler));

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

            builder.Services.AddMvc().AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Add(new InterceptedEmbeddedFileProvider(typeof(IListableContent).Assembly));
            });

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