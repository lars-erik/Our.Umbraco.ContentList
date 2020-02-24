using System;
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
            // TODO: Validate longs don't surpass ints (?)

            if (contentContext == null)
                return new List<IListableContent>().AsQueryable();
            var listables = contentContext.Children.OfType<IListableContent>().Where(c => c.IsVisible());
            listables = ListableSorting.Apply(listables, queryParameters.CustomParameters);
            listables = listables.Skip((int)pagingParameter.PreSkip).Skip((int)pagingParameter.Skip).Take((int)pagingParameter.Take);
            return listables.AsQueryable();
        }

        public long Count(long preSkip)
        {
            if (preSkip > int.MaxValue)
            {
                throw new Exception("Child lists does not support skipping more than 32 bit values");
            }
            return contentContext == null 
                ? 0 
                : contentContext.Children.Skip((int)preSkip).Count();
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
