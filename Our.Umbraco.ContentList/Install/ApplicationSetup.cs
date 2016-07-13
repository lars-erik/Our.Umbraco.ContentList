using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Our.Umbraco.ContentList.Web;
using Our.Umbraco.ContentList.Web.DataSources;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;

namespace Our.Umbraco.ContentList.Install
{
    public class ApplicationSetup : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

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
