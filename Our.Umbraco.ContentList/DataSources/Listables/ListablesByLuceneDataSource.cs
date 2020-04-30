using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Examine.Search;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    [DataSourceMetadata(typeof(ListablesByLuceneDataSourceMetadata))]
    public class ListablesByLuceneDataSource : IListableDataSource
    {
        private readonly LuceneSearcher searcher;
        private IPublishedContentCache contentCache;

        public ListablesByLuceneDataSource(IExamineManager examineManager, IPublishedSnapshotAccessor snapshotAccessor)
        {
            examineManager.TryGetIndex("ExternalIndex", out var index);
            searcher = (LuceneSearcher)index.GetSearcher();
            contentCache = snapshotAccessor.PublishedSnapshot.Content;
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            var searchResults = Search(query);
            var result = searchResults
                .Skip((int)queryPaging.PreSkip)
                .Skip((int)queryPaging.Skip)
                .Take((int)queryPaging.Take)
                .Select(GetContent)
                .OfType<IListableContent>()
                .AsQueryable()
                ;
            return result;
        }

        public long Count(ContentListQuery query, long preSkip)
        {
            return Search(query).TotalItemCount - preSkip;
        }

        private IPublishedContent GetContent(ISearchResult r)
        {
            return contentCache.GetById(Convert.ToInt32(r.Id));
        }

        private ISearchResults Search(ContentListQuery query)
        {
            // Fields?
            var fullstringKey = query.CustomParameters["queryParameter"];
            var phrase = query.HttpContext.Request.QueryString[fullstringKey];
            var showIfNoPhrase = query.CustomParameters.ContainsKey("showIfNoPhrase")
                                && query.CustomParameters["showIfNoPhrase"] == "1";
            var hasPhrase = !String.IsNullOrWhiteSpace(phrase);
            var shouldShow = showIfNoPhrase || hasPhrase;
            var controlQuery = query.CustomParameters["query"];

            // Build lucene query
            if (shouldShow)
            {
                var booleanQuery = CreateControlQuery(controlQuery);
                if (hasPhrase)
                {
                    booleanQuery = CreatePhraseQuery(booleanQuery, phrase);
                }

                var searchResults = booleanQuery.Execute();
                return searchResults;
            }

            return EmptySearchResults.Instance;
        }

        protected virtual IBooleanOperation CreateControlQuery(string controlQuery)
        {
            return searcher.CreateQuery().NativeQuery(controlQuery);
        }

        protected virtual IBooleanOperation CreatePhraseQuery(IBooleanOperation luceneQuery, string phrase)
        {
            return luceneQuery.And()
                .GroupedOr(
                    searcher.GetAllIndexedFields(), 
                    phrase.Split(' ').Select(x => (IExamineValue)new ExamineValue(Examineness.Boosted, x, 1.5f))
                        .Union(
                            phrase.Split(' ').Select(x => (IExamineValue)new ExamineValue(Examineness.SimpleWildcard, x + "*"))
                        )
                        .ToArray()
                );
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
                    View = "/Umbraco/views/propertyeditors/textarea/textarea.html"
                },
                new DataSourceParameterDefinition
                {
                    Key = "queryParameter",
                    Label = "Fulltext Querystring Parameter",
                    View = "/Umbraco/views/propertyeditors/textbox/textbox.html"
                },
                new DataSourceParameterDefinition
                {
                    Key = "showIfNoPhrase",
                    Label = "Show if no querystring parameter",
                    View = "/Umbraco/views/prevalueeditors/boolean.html"
                }
            };
        }
    }
}
