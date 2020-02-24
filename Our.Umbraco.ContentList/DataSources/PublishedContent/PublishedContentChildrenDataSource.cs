using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
//using Umbraco.Web;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentChildrenMetadata))]
    public class PublishedContentChildrenDataSource : IPublishedContentDataSource
    {
        private readonly IPublishedContent contentContext;
        private readonly QueryParameters queryParameters;

        public PublishedContentChildrenDataSource(QueryParameters queryParameters)
        {
            this.contentContext = queryParameters.ContextContent;
            this.queryParameters = queryParameters;
        }

        public IQueryable<IPublishedContent> Query(PagingParameter pagingParameter)
        {
            // TODO: ints?
            if (contentContext == null)
                return new List<IPublishedContent>().AsQueryable();
            var listables = contentContext.Children.Where(c => true); // TODO: Where did .IsVisible() go?
            listables = PublishedContentSorting.Apply(listables, queryParameters.CustomParameters);
            listables = listables.Skip((int)pagingParameter.PreSkip).Skip((int)pagingParameter.Skip).Take((int)pagingParameter.Take);
            return listables.AsQueryable();
        }

        public long Count(long preSkip)
        {
            // TODO: Validate int
            return contentContext == null
                ? 0
                : contentContext.Children.Skip((int)preSkip).Count();
        }
    }

    public class PublishedContentChildrenMetadata : DataSourceMetadata
    {
        public PublishedContentChildrenMetadata()
        {
            Key = typeof(PublishedContentChildrenDataSource).GetFullNameWithAssembly();
            Name = "List of children";
            Parameters = new List<DataSourceParameterDefinition>
            {
                PublishedContentSorting.Parameter
            };
        }
    }

}
