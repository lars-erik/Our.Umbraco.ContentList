using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Parameters;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.DataSources
{
    public class ChildrenOfMultipleDataSource : IListableDataSource
    {
        private IEnumerable<IListableContent> cachedResult;
        private readonly IPublishedContentCache cache;

        public ChildrenOfMultipleDataSource(IPublishedSnapshotAccessor accessor)
        {
            if (accessor.TryGetPublishedSnapshot(out var snapshot))
            { 
                cache = snapshot.Content;
            }
            else
            {
                throw new Exception("Can't get published cache from snapshot accessor");
            }
        }

        public async Task<IQueryable<IListableContent>> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            var result = (await BaseQuery(query, (int)queryPaging.PreSkip))
                .Skip((int)queryPaging.Skip)
                .Take((int)queryPaging.Take)
                .AsQueryable();
            return result;
        }

        public async Task<long> Count(ContentListQuery query, long preSkip)
        {
            var baseResult = await BaseQuery(query, (int)preSkip);
            return baseResult.Count();
        }

        private async Task<IEnumerable<IListableContent>> BaseQuery(ContentListQuery query, int preSkip)
        {
            if (cachedResult == null)
            {
                var result = (query.CustomParameter<string>("nodes") ?? "")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(id => cache.GetById(Convert.ToInt32(id)).Children.OfType<IListableContent>())
                    .ApplySorting(query.CustomParameters)
                    .Skip(preSkip);
                cachedResult = result;
            }
            return await Task.FromResult(cachedResult);
        }
    }

    public class ChildrenOfMultipleDataSourceMetadata : DataSourceMetadata<ChildrenOfMultipleDataSource>
    {
        public override string Name => "Children from individually selected parents";

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

    public class TreePickerDataSourceConfig : DataSourceConfig
    {
        [JsonProperty("multiPicker")]
        public bool MultiPicker { get; set; }
        [JsonProperty("maxNumber")]
        public int MaxNumber { get; set; }
    }
}
