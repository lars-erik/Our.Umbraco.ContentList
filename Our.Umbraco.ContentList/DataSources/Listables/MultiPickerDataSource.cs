using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(MultiPickerDataSourceMetadata))]
    public class MultiPickerDataSource : IListableDataSource

    {
        private IPublishedContentCache contentCache;
        private IEnumerable<IListableContent> cachedResult;

        public MultiPickerDataSource(IPublishedSnapshotAccessor snapshotAccessor)
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
            return BaseQuery(query, (int)preSkip).Count();
        }

        private IEnumerable<IListableContent> BaseQuery(ContentListQuery query, int preSkip)
        {
            if (cachedResult == null)
            {
                cachedResult = (query.CustomParameters["nodes"] ?? "")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => contentCache.GetById(Convert.ToInt32(id)))
                    .OfType<IListableContent>()
                    .Skip(preSkip);
            }
            return cachedResult;
        }
    }

    public class MultiPickerDataSourceMetadata : DataSourceMetadata
    {
        public MultiPickerDataSourceMetadata()
        {
            Key = typeof(MultiPickerDataSource).GetFullNameWithAssembly();
            Name = "Individually selected content";
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "nodes",
                    Label = "Content",
                    View = "/Umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
                    Config = new TreePickerDataSourceConfig
                    {
                        MultiPicker = true,
                        MaxNumber = 0
                    }
                },
                ListableSorting.Parameter
            };
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
