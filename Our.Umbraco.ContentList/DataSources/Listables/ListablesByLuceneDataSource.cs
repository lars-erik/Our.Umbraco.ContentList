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
            var luceneQuery = searcher.CreateQuery().NativeQuery(query.CustomParameters["query"]);
            var searchResults = luceneQuery.Execute();
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
                    View = "/Umbraco/views/propertyeditors/textbox/textbox.html"
                }
            };
        }
    }
}
