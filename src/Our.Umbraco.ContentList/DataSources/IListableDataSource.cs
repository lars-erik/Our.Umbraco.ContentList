using System.Linq;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.DataSources
{
    public interface IListableDataSource
    {
        Task<IQueryable<IListableContent>> Query(ContentListQuery query, QueryPaging queryPaging);
        Task<long> Count(ContentListQuery query, long preSkip);
    }
}