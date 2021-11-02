using System.Linq;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.DataSources
{
    public interface IListableDataSource
    {
        IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging);
        long Count(ContentListQuery query, long preSkip);
    }
}