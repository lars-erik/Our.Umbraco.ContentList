using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentChildrenMetadata))]
    public class PublishedContentChildrenDataSource
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
            if (contentContext == null)
                return new List<IPublishedContent>().AsQueryable();
            var listables = contentContext.Children.Where(c => c.IsVisible());
            listables = PublishedContentSorting.Apply(listables, queryParameters.CustomParameters);
            listables = listables.Skip(pagingParameter.PreSkip).Skip(pagingParameter.Skip).Take(pagingParameter.Take);
            return listables.AsQueryable();
        }

        public int Count(int preSkip)
        {
            return contentContext == null
                ? 0
                : contentContext.Children.Skip(preSkip).Count();
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
