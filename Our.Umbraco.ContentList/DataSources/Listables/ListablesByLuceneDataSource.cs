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
        protected readonly IExamineManager ExamineManager;
        protected LuceneSearcher Searcher = null;
        protected IPublishedContentCache ContentCache;

        public ListablesByLuceneDataSource(IExamineManager examineManager, IPublishedSnapshotAccessor snapshotAccessor)
        {
            this.ExamineManager = examineManager;
            ContentCache = snapshotAccessor.PublishedSnapshot.Content;
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
                .ToList()
                .AsQueryable()
                ;
            return result;
        }

        public long Count(ContentListQuery query, long preSkip)
        {
            return Search(query).TotalItemCount - preSkip;
        }

        protected virtual object GetContent(ISearchResult r)
        {
            return ContentCache.GetById(Convert.ToInt32(r.Id));
        }

        private ISearchResults Search(ContentListQuery query)
        {
            if (Searcher == null)
            {
                ExamineManager.TryGetIndex(query.CustomParameters["index"].IfNullOrWhiteSpace("ExternalIndex"), out var index);
                Searcher = (LuceneSearcher)index.GetSearcher();
            }

            // Fields?
            var fullstringKey = query.CustomParameters["queryParameter"];
            var phrase = query.HttpContext.Request.QueryString[fullstringKey];
            var showIfNoPhrase = query.CustomParameters.ContainsKey("showIfNoPhrase")
                                && query.CustomParameters["showIfNoPhrase"] == "1";
            var noFullStringKey = String.IsNullOrEmpty(fullstringKey);
            var hasPhrase = !String.IsNullOrWhiteSpace(phrase);
            var shouldShow = showIfNoPhrase || hasPhrase || noFullStringKey;
            var controlQuery = query.CustomParameters["query"];

            // Build lucene query
            if (shouldShow)
            {
                IQueryExecutor booleanQuery = CreateControlQuery(controlQuery);
                if (hasPhrase)
                {
                    booleanQuery = CreatePhraseQuery((IBooleanOperation)booleanQuery, phrase);
                }

                var searchResults = booleanQuery.Execute();
                return searchResults;
            }

            return EmptySearchResults.Instance;
        }

        protected virtual IBooleanOperation CreateControlQuery(string controlQuery)
        {
            return Searcher.CreateQuery().NativeQuery(controlQuery);
        }

        protected virtual IQueryExecutor CreatePhraseQuery(IBooleanOperation luceneQuery, string phrase)
        {
            return luceneQuery.And()
                .GroupedOr(
                    Searcher.GetAllIndexedFields(),
                    phrase.Split(' ').Select(x => (IExamineValue)new ExamineValue(Examineness.Boosted, x, 1.5f))
                        .Union(
                            phrase.Split(' ').Select(x => (IExamineValue)new ExamineValue(Examineness.ComplexWildcard, x))
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
            Key = typeof(ListablesByLuceneDataSource).GetFullNameWithAssembly();
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
                    Key = "index",
                    Label = "Examine Index Name",
                    View = "/Umbraco/views/propertyeditors/textbox/textbox.html"
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
