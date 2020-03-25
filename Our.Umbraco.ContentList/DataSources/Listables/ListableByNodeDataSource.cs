using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Composing;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListableByNodeMetadata))]
    public class ListableByNodeDataSource : IListableDataSource

    {
        private readonly IPublishedContent contentContext;
        private readonly QueryParameters queryParameters;

        public ListableByNodeDataSource(QueryParameters queryParameters)
        {
            this.contentContext = queryParameters.ContextContent;
            this.queryParameters = queryParameters;
        }

        public IQueryable<IListableContent> Query(PagingParameter pagingParameter)
        {
            return BaseQuery((int)pagingParameter.PreSkip)
                .Skip((int)pagingParameter.Skip)
                .Take((int)pagingParameter.Take)
                .AsQueryable();

        }

        public long Count(long preSkip)
        {
            return BaseQuery((int) preSkip).Count();
        }

        private IEnumerable<IListableContent> BaseQuery(int preSkip)
        {
            if (Udi.TryParse(queryParameters.CustomParameters["parentNode"], out var contentId))
            {
                var content = Current.PublishedSnapshot.Content.GetById(contentId);
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
