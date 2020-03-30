using System.Linq;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    public interface IListableDataSource
    {
        IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging);
        long Count(ContentListQuery query, long preSkip);
    }
}