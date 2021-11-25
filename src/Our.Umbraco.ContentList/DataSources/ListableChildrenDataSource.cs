using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Parameters;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.DataSources
{
    public class ListableChildrenDataSource : IListableDataSource
    {
        public ListableChildrenDataSource()
        {
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging queryPaging)
        {
            // TODO: Validate longs don't surpass ints (?)

            if (query.ContextContent == null)
                return new List<IListableContent>().AsQueryable();

            var culture = LanguageParameter.Culture(query);
            var listables = query.ContextContent.Children(culture).OfType<IListableContent>().Where(c => c.IsVisible());
            listables = ListableSorting.ApplySorting(listables, query.CustomParameters);
            listables = listables.Skip((int)queryPaging.PreSkip).Skip((int)queryPaging.Skip).Take((int)queryPaging.Take);
            return listables.AsQueryable();
        }

        public long Count(ContentListQuery query, long preSkip)
        {
            if (preSkip > int.MaxValue)
            {
                throw new Exception("Child lists does not support skipping more than 32 bit values");
            }
            return query.ContextContent?.Children.Skip((int)preSkip).Count() ?? 0;
        }
    }

    public class ListableChildrenMetadata : DataSourceMetadata<ListableChildrenDataSource>
    {
        private readonly ILocalizationService localizationService;
        private DataSourceParameterDefinition languageParameter;
        private bool loadLanguage = true;

        public ListableChildrenMetadata(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        public override string Name => "List of children";

        public override IEnumerable<DataSourceParameterDefinition> Parameters
        {
            get
            {
                yield return ListableSorting.Parameter;
                if (UseLanguageParameter())
                {
                    yield return languageParameter;
                }
            }
        }

        private bool UseLanguageParameter()
        {
            if (loadLanguage)
            {
                languageParameter = LanguageParameter.Create(localizationService);
                loadLanguage = false;
            }

            return languageParameter != null;
        }
    }
}
