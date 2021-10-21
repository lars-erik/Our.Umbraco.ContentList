using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.DataSources;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
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

            builder.AddNotificationHandler<ServerVariablesParsingNotification, AddControllersToApiPaths>();
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