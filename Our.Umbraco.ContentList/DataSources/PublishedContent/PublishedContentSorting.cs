using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.DataSources.PublishedContent
{
    public class PublishedContentSorting
    {
        public static readonly DataSourceParameterDefinition Parameter = new DataSourceParameterDefinition
        {
            Key = "sort",
            Label = "Sort by",
            View = "/umbraco/views/propertyeditors/dropdown/dropdown.html",
            Config = new DataSourceConfig
            {
                Items = new[]
                {
                    new DataSourceParameterOption {Id = "sortorder", Value = "Sort order"},
                    new DataSourceParameterOption {Id = "dateasc", Value = "Date"},
                    new DataSourceParameterOption {Id = "datedesc", Value = "Date descending"},
                }
            }
        };

        public static readonly Dictionary<string, Func<IEnumerable<IPublishedContent>, IEnumerable<IPublishedContent>>> Sorters = new Dictionary<string, Func<IEnumerable<IPublishedContent>, IEnumerable<IPublishedContent>>>
        {
            {"sortorder", list => list.OrderBy(c => c.SortOrder)},
            {"dateasc", list => list.OrderBy(ContentDate)},
            {"datedesc", list => list.OrderByDescending(ContentDate)},
        };

        public static IEnumerable<IPublishedContent> Apply(IEnumerable<IPublishedContent> listables, IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("sort") && Sorters.ContainsKey(parameters["sort"]))
            {
                listables = Sorters[parameters["sort"]](listables);
            }
            return listables;
        }

        public static DateTime ContentDate(IPublishedContent c)
        {
            return c.CreateDate;
        }
    }
}