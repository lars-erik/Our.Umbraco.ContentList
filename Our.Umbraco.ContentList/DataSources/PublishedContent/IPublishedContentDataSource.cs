using System.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    public interface IPublishedContentDataSource
    {
        IQueryable<IPublishedContent> Query(QueryPaging queryPaging);
        long Count(long preSkip);
    }
}