using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
//using Umbraco.Web;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentChildrenMetadata))]
    public class PublishedContentChildrenDataSource : IPublishedContentDataSource
    {
        private readonly IPublishedContent contentContext;
        private readonly ContentListQuery query;

        public PublishedContentChildrenDataSource(ContentListQuery query)
        {
            this.contentContext = query.ContextContent;
            this.query = query;
        }

        public IQueryable<IPublishedContent> Query(QueryPaging queryPaging)
        {
            // TODO: ints?
            if (contentContext == null)
                return new List<IPublishedContent>().AsQueryable();
            var listables = contentContext.Children.Where(c => true); // TODO: Where did .IsVisible() go?
            listables = PublishedContentSorting.Apply(listables, query.CustomParameters);
            listables = listables.Skip((int)queryPaging.PreSkip).Skip((int)queryPaging.Skip).Take((int)queryPaging.Take);
            return listables.AsQueryable();
        }

        public long Count(long preSkip)
        {
            // TODO: Validate int
            return contentContext == null
                ? 0
                : contentContext.Children.Skip((int)preSkip).Count();
        }
    }

    public class PublishedContentChildrenMetadata : DataSourceMetadata
    {
        public PublishedContentChildrenMetadata()
        {
            Key = typeof(PublishedContentChildrenDataSource).GetFullNameWithAssembly();
            Name = "List of children";
            Parameters = new List<DataSourceParameterDefinition>
            {
                PublishedContentSorting.Parameter
            };
        }
    }

}
