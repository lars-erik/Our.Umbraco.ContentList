﻿using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
//using Umbraco.Web;
//using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    [DataSourceMetadata(typeof(PublishedContentByXPathDataSourceMetadata))]
    public class PublishedContentByXPathDataSource : IPublishedContentDataSource
    {
        private readonly QueryParameters parameters;
        //private readonly UmbracoContext ctx;
        //private readonly ContextualPublishedCache<IPublishedContentCache> contentService;

        public PublishedContentByXPathDataSource(QueryParameters parameters)
        {
            this.parameters = parameters;
            throw new NotImplementedException("Uses obsolete methods");
            //ctx = UmbracoContext.Current;
            //contentService = ctx.ContentCache;
        }

        public IQueryable<IPublishedContent> Query(PagingParameter pagingParameter)
        {
            // TODO: Ints?
            var listables = Query();
            listables = PublishedContentSorting.Apply(listables, parameters.CustomParameters);
            listables = listables.Skip((int)pagingParameter.PreSkip).Skip((int)pagingParameter.Skip).Take((int)pagingParameter.Take);
            return listables.AsQueryable();
        }

        public long Count(long preSkip)
        {
            return Query().Count() - preSkip;
        }

        private IEnumerable<IPublishedContent> Query()
        {
            throw new NotImplementedException("Uses obsolete methods");
            //var contents = contentService.GetByXPath(parameters.CustomParameters["xpath"]);
            //var listables = contents.Where(c => c.IsVisible());
            //return listables;
        }
    }

    public class PublishedContentByXPathDataSourceMetadata : DataSourceMetadata
    {
        public PublishedContentByXPathDataSourceMetadata()
        {
            Name = "XPath Query";
            Key = typeof(PublishedContentByXPathDataSource).GetFullNameWithAssembly();
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "xpath",
                    Label = "Query",
                    View = "textstring"
                },
                PublishedContentSorting.Parameter
            };
        }
    }
}
