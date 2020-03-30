using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Our.Umbraco.ContentList.Web;
using Our.Umbraco.ContentList.Web.DataSources;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace Our.Umbraco.ContentList.Install
{
    public class ServerVariablesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            ServerVariablesParser.Parsing += AddApiPaths;
        }

        private void AddApiPaths(object sender, Dictionary<string, object> e)
        {
            var umbracoUrls = ((Dictionary<string, object>)e["umbracoUrls"]);

            umbracoUrls.Add(
                "Our.Umbraco.ContentList.Web.DataSources.ListableDataSource",
                new UrlHelper(HttpContext.Current.Request.RequestContext).GetUmbracoApiServiceBaseUrl<ListableDataSourceController>(c => c.GetDataSources())
                );

            umbracoUrls.Add(
                "Our.Umbraco.ContentList.Web.ContentListApi",
                new UrlHelper(HttpContext.Current.Request.RequestContext).GetUmbracoApiServiceBaseUrl<ContentListApiController>(c => c.ListTemplates())
            );


        }
    }
}
