using System.Collections.Generic;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.ContentList.Web.DataSources
{
    [PluginController("OurContentList")]
    public class ListableDataSourceController : UmbracoAuthorizedJsonController
    {
        public List<DataSourceMetadata> GetDataSources()
        {
            return ListableDataSourceFactory.GetDataSources();
        }
    }
}
