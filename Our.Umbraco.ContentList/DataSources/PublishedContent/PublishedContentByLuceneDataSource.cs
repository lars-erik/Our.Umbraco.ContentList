using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentByLuceneDataSourceMetadata))]
    public class PublishedContentByLuceneDataSource : IPublishedContentDataSource
    {
        private readonly QueryParameters parameters;
        private readonly UmbracoContext ctx;
        private readonly LuceneSearcher searcher;

        public PublishedContentByLuceneDataSource(QueryParameters parameters)
        {
            this.parameters = parameters;
            ctx = UmbracoContext.Current;
            searcher = (LuceneSearcher)ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
        }

        public IQueryable<IPublishedContent> Query(PagingParameter pagingParameter)
        {
            var searchResults = Search();
            var result = searchResults
                .Skip(pagingParameter.PreSkip)
                .Skip(pagingParameter.Skip)
                .Take(pagingParameter.Take)
                .Select(GetContent)
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

    public class PublishedContentByLuceneDataSourceMetadata : DataSourceMetadata
    {
        public PublishedContentByLuceneDataSourceMetadata()
        {
            Name = "Lucene Query";
            Key = typeof(PublishedContentByLuceneDataSource).GetFullNameWithAssembly();
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
