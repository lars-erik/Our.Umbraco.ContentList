using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Web.Composing;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListableByNodeMetadata))]
    public class ListableByNodeDataSource : IListableDataSource

    {
        private IPublishedContentCache contentCache;

        public ListableByNodeDataSource(IPublishedSnapshotAccessor snapshotAccessor)
        {
            this.contentCache = snapshotAccessor.PublishedSnapshot.Content;
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            return BaseQuery(query, (int)queryPaging.PreSkip)
                .Skip((int)queryPaging.Skip)
                .Take((int)queryPaging.Take)
                .AsQueryable();

        }

        public long Count(ContentListQuery query, long preSkip)
        {
            return BaseQuery(query, (int) preSkip).Count();
        }

        private IEnumerable<IListableContent> BaseQuery(ContentListQuery query, int preSkip)
        {
            if (Udi.TryParse(query.CustomParameter<string>("parentNode"), out var contentId))
            {
                // TODO: Inject
                var content = contentCache.GetById(contentId);
                return content
                    .Children
                    .OfType<IListableContent>()
                    .Skip(preSkip);
            }
            return Enumerable.Empty<IListableContent>().AsQueryable();
        }
    }

    public class ListableByNodeMetadata : DataSourceMetadata
    {
        public ListableByNodeMetadata()
        {
            Key = typeof(ListableByNodeDataSource).GetFullNameWithAssembly();
            Name = "List of items from tree";
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "parentNode",
                    Label = "From node",
                    View = "/Umbraco/Views/prevalueeditors/treepicker.html"
                },
                ListableSorting.Parameter
            };
        }

    }
}
