using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListablesByLuceneDataSourceMetadata))]
    public class ListablesByLuceneDataSource : IListableDataSource
    {
        private readonly QueryParameters parameters;
        private readonly UmbracoContext ctx;
        private readonly LuceneSearcher searcher;

        public ListablesByLuceneDataSource(QueryParameters parameters)
        {
            this.parameters = parameters;
            ctx = UmbracoContext.Current;
            searcher = (LuceneSearcher)ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
        }

        public IQueryable<IListableContent> Query(PagingParameter pagingParameter)
        {
            var searchResults = Search();
            var result = searchResults
                .Skip(pagingParameter.PreSkip)
                .Skip(pagingParameter.Skip)
                .Take(pagingParameter.Take)
                .Select(GetContent)
                .OfType<IListableContent>()
                .AsQueryable()
                ;
            return result;
        }

        public int Count(int preSkip)
        {
            return Search().TotalItemCount - preSkip;
        }

        private IPublishedContent GetContent(SearchResult r)
        {
            Debug.WriteLine("Fetched document id {0}", r.Id);
            return ctx.ContentCache.GetById(r.Id);
        }

        private ISearchResults Search()
        {
            var query = searcher.CreateSearchCriteria().RawQuery(parameters.CustomParameters["query"]);
            var searchResults = searcher.Search(query);
            return searchResults;
        }
    }

    public class ListablesByLuceneDataSourceMetadata : DataSourceMetadata
    {
        public ListablesByLuceneDataSourceMetadata()
        {
            Name = "Lucene Query";
            Key = typeof (ListablesByLuceneDataSource).GetFullNameWithAssembly();
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "query",
                    Label = "Query",
                    View = "textstring"
                }
            };
        }
    }
}
