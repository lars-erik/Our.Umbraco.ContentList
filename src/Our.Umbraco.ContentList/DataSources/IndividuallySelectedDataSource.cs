using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Parameters;
using Umbraco.Cms.Core.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources
{
    public class IndividuallySelectedDataSource : IListableDataSource
    {
        private readonly IPublishedContentCache contentCache;

        public IndividuallySelectedDataSource(IPublishedSnapshotAccessor snapshotAccessor)
        {
            snapshotAccessor.TryGetPublishedSnapshot(out var snapshot);
            contentCache = snapshot.Content;
        }

        public async Task<IQueryable<IListableContent>> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            var listables = BaseQuery(query);
            var result = listables.AsQueryable();
            return await Task.FromResult(result);
        }

        public async Task<long> Count(ContentListQuery query, long preSkip)
        {
            return await Task.FromResult(BaseQuery(query).Count());
        }

        private IEnumerable<IListableContent> BaseQuery(ContentListQuery query)
        {
            var nodes = ((string) query.CustomParameters["nodes"]).Split(',').Select(x => Convert.ToInt32(x));
            var content = nodes.Select(x => contentCache.GetById(x));
            var listables = content.OfType<IListableContent>();
            return listables;
        }
    }

    public class IndividuallySelectedDataSourceMetadata : DataSourceMetadata<IndividuallySelectedDataSource>
    {
        public override string Name => "Individually selected nodes";

        private static readonly DataSourceParameterDefinition NodesParameterDefinition = new()
        {
            Key = "nodes",
            Label = "Content",
            View = "/Umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
            Config = new TreePickerDataSourceConfig
            {
                MultiPicker = true,
                MaxNumber = 0
            }
        };

        public override IEnumerable<DataSourceParameterDefinition> Parameters
        {
            get
            {
                yield return NodesParameterDefinition;
                yield return ListableSorting.Parameter;
            }

        }
    }

}
