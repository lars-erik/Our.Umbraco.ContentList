using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListableChildrenMetadata))]
    public class ListableChildrenDataSource : IListableDataSource
    {
        public ListableChildrenDataSource()
        {
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            // TODO: Validate longs don't surpass ints (?)

            if (query.ContextContent == null)
                return new List<IListableContent>().AsQueryable();
            var listables = query.ContextContent.Children.OfType<IListableContent>().Where(c => c.IsVisible());
            listables = ListableSorting.Apply(listables, query.CustomParameters);
            listables = listables.Skip((int)queryPaging.PreSkip).Skip((int)queryPaging.Skip).Take((int)queryPaging.Take);
            return listables.AsQueryable();
        }

        public long Count(ContentListQuery query, long preSkip)
        {
            if (preSkip > int.MaxValue)
            {
                throw new Exception("Child lists does not support skipping more than 32 bit values");
            }
            return query.ContextContent == null 
                ? 0 
                : query.ContextContent.Children.Skip((int)preSkip).Count();
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
