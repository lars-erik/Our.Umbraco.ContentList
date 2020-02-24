using System.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    public interface IPublishedContentDataSource
    {
        IQueryable<IPublishedContent> Query(PagingParameter pagingParameter);
        long Count(long preSkip);
    }
}