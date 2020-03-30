using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

//using Umbraco.Web;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentByLuceneDataSourceMetadata))]
    public class PublishedContentByLuceneDataSource : IPublishedContentDataSource
    {
        private readonly ContentListQuery parameters;
        //private readonly UmbracoContext ctx;
        private readonly LuceneSearcher searcher;

        public PublishedContentByLuceneDataSource(ContentListQuery parameters)
        {
            this.parameters = parameters;

            throw new NotImplementedException("Uses obsolete methods");
            //ctx = UmbracoContext.Current;
            //searcher = (LuceneSearcher)ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
        }

        public IQueryable<IPublishedContent> Query(QueryPaging queryPaging)
        {
            // TODO: Ints?

            var searchResults = Search();
            var result = searchResults
                .Skip((int)queryPaging.PreSkip)
                .Skip((int)queryPaging.Skip)
                .Take((int)queryPaging.Take)
                .Select(GetContent)
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
