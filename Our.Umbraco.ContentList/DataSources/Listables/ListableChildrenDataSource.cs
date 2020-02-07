using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListableChildrenMetadata))]
    public class ListableChildrenDataSource : IListableDataSource
    {
        private readonly IPublishedContent contentContext;
        private readonly QueryParameters queryParameters;

        public ListableChildrenDataSource(QueryParameters queryParameters)
        {
            this.contentContext = queryParameters.ContextContent;
            this.queryParameters = queryParameters;
        }

        public IQueryable<IListableContent> Query(PagingParameter pagingParameter)
        {
            if (contentContext == null)
                return new List<IListableContent>().AsQueryable();
            var listables = contentContext.Children.OfType<IListableContent>().Where(c => c.IsVisible());
            listables = ListableSorting.Apply(listables, queryParameters.CustomParameters);
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



    public class ListableChildrenMetadata : DataSourceMetadata
    {
        public ListableChildrenMetadata()
        {
            Key = typeof(ListableChildrenDataSource).GetFullNameWithAssembly();
            Name = "List of children";
            Parameters = new List<DataSourceParameterDefinition>
            {
                ListableSorting.Parameter
            };
        }
    }
}
