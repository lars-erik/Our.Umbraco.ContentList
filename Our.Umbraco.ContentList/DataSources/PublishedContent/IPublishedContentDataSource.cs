using System.Linq;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core.Models;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    public interface IPublishedContentDataSource
    {
        IQueryable<IPublishedContent> Query(PagingParameter pagingParameter);
        int Count(int preSkip);
    }
}