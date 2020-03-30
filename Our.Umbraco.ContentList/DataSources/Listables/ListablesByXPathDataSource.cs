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
        private readonly IPublishedContentCache contentCache;

        public ListablesByXPathDataSource(IPublishedSnapshotAccessor snapshotAccessor)
        {
            contentCache = snapshotAccessor.PublishedSnapshot.Content;
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            var listables = Query(query);
            listables = ListableSorting.Apply(listables, query.CustomParameters);
            listables = listables.Skip((int)queryPaging.PreSkip).Skip((int)queryPaging.Skip).Take((int)queryPaging.Take);
            return listables.AsQueryable();
        }

        public long Count(ContentListQuery query, long preSkip)
        {
            return Query(query).Count() - preSkip;
        }

        private IEnumerable<IListableContent> Query(ContentListQuery query)
        {
            var contents = contentCache.GetByXPath(query.CustomParameters["xpath"]);
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
