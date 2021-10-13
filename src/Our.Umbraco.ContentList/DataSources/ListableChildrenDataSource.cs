using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Parameters;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.DataSources
{
    public class ListableChildrenDataSource
    {
        public ListableChildrenDataSource()
        {
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            // TODO: Validate longs don't surpass ints (?)

            if (query.ContextContent == null)
                return new List<IListableContent>().AsQueryable();

            var culture = LanguageParameter.Culture(query);
            var listables = query.ContextContent.Children(culture).OfType<IListableContent>().Where(c => c.IsVisible());
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
}
