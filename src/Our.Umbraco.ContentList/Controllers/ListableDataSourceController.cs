using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Our.Umbraco.ContentList.Controllers
{
    [PluginController("OurContentList")]
    public class ListableDataSourceController : UmbracoAuthorizedJsonController
    {
        //public List<DataSourceMetadata> GetDataSources()
        //{
        //    return ListableDataSourceFactory.GetDataSources();
        //}
    }
}
