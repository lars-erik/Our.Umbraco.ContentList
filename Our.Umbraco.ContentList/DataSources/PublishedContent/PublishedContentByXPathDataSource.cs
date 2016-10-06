using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentByXPathDataSourceMetadata))]
    public class PublishedContentByXPathDataSource : IPublishedContentDataSource
    {
        private readonly QueryParameters parameters;
        private readonly UmbracoContext ctx;
        private readonly ContextualPublishedCache<IPublishedContentCache> contentService;

        public PublishedContentByXPathDataSource(QueryParameters parameters)
        {
            this.parameters = parameters;
            ctx = UmbracoContext.Current;
            contentService = ctx.ContentCache;
        }

        public IQueryable<IPublishedContent> Query(PagingParameter pagingParameter)
        {
            var listables = Query();
            listables = PublishedContentSorting.Apply(listables, parameters.CustomParameters);
            listables = listables.Skip(pagingParameter.PreSkip).Skip(pagingParameter.Skip).Take(pagingParameter.Take);
            return listables.AsQueryable();
        }

        public int Count(int preSkip)
        {
            return Query().Count() - preSkip;
        }

        private IEnumerable<IPublishedContent> Query()
        {
            var contents = contentService.GetByXPath(parameters.CustomParameters["xpath"]);
            var listables = contents.Where(c => c.IsVisible());
            return listables;
        }
    }

    public class PublishedContentByXPathDataSourceMetadata : DataSourceMetadata
    {
        public PublishedContentByXPathDataSourceMetadata()
        {
            Name = "XPath Query";
            Key = typeof(PublishedContentByXPathDataSource).GetFullNameWithAssembly();
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "xpath",
                    Label = "Query",
                    View = "textstring"
                },
                PublishedContentSorting.Parameter
            };
        }
    }
}
