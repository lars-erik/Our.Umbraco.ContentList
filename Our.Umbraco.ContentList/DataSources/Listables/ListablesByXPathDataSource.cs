using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.PublishedCache;

//using Umbraco.Web;
//using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListablesByXPathDataSourceMetadata))]
    public class ListablesByXPathDataSource : IListableDataSource
    {
        private readonly QueryParameters parameters;
        private readonly UmbracoContext ctx;
        private readonly IPublishedContentCache contentCache;

        public ListablesByXPathDataSource(QueryParameters parameters)
        {
            this.parameters = parameters;

            ctx = Current.UmbracoContext;
            contentCache = ctx.Content;
        }

        public IQueryable<IListableContent> Query(PagingParameter pagingParameter)
        {
            var listables = Query();
            listables = ListableSorting.Apply(listables, parameters.CustomParameters);
            listables = listables.Skip((int)pagingParameter.PreSkip).Skip((int)pagingParameter.Skip).Take((int)pagingParameter.Take);
            return listables.AsQueryable();
        }

        public long Count(long preSkip)
        {
            return Query().Count() - preSkip;
        }

        private IEnumerable<IListableContent> Query()
        {
            var contents = contentCache.GetByXPath(parameters.CustomParameters["xpath"]);
            var listables = contents.OfType<IListableContent>().Where(c => c.IsVisible());
            return listables;
        }
    }

    public class ListablesByXPathDataSourceMetadata : DataSourceMetadata
    {
        public ListablesByXPathDataSourceMetadata()
        {
            Name = "XPath Query";
            Key = typeof (ListablesByXPathDataSource).GetFullNameWithAssembly();
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "xpath",
                    Label = "Query",
                    View = "/Umbraco/views/propertyeditors/textbox/textbox.html"
                },
                ListableSorting.Parameter
            };
        }
    }
}
