using System.Linq;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    public interface IListableDataSource
    {
        IQueryable<IListableContent> Query(PagingParameter pagingParameter);
        int Count(int preSkip);
    }
}