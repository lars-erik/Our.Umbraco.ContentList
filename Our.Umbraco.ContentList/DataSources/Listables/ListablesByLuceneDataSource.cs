using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListablesByLuceneDataSourceMetadata))]
    public class ListablesByLuceneDataSource : IListableDataSource
    {
        private readonly QueryParameters parameters;
        //private readonly UmbracoContext ctx;
        private readonly LuceneSearcher searcher;

        public ListablesByLuceneDataSource(QueryParameters parameters)
        {
            throw new NotImplementedException("Uses obsolete methods");
            //this.parameters = parameters;
            //ctx = UmbracoContext.Current;
            //searcher = (LuceneSearcher)ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
        }

        public IQueryable<IListableContent> Query(PagingParameter pagingParameter)
        {
            // TODO: Validate ints?

            var searchResults = Search();
            var result = searchResults
                .Skip((int)pagingParameter.PreSkip)
                .Skip((int)pagingParameter.Skip)
                .Take((int)pagingParameter.Take)
                .Select(GetContent)
                .OfType<IListableContent>()
                .AsQueryable()
                ;
            return result;
        }

        public long Count(long preSkip)
        {
            return Search().TotalItemCount - preSkip;
        }

        private IPublishedContent GetContent(ISearchResult r)
        {
            throw new NotImplementedException("Uses obsolete methods");

            //Debug.WriteLine("Fetched document id {0}", r.Id);
            //return ctx.ContentCache.GetById(r.Id);
        }

        private ISearchResults Search()
        {
            throw new NotImplementedException("Uses obsolete methods");
            //var query = searcher.CreateSearchCriteria().RawQuery(parameters.CustomParameters["query"]);
            //var searchResults = searcher.Search(query);
            //return searchResults;
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
