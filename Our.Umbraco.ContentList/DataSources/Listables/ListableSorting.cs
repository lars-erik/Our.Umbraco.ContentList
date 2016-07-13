using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    public class ListableSorting
    {
        public static readonly DataSourceParameterDefinition Parameter = new DataSourceParameterDefinition
        {
            Key = "sort",
            Label = "Sort by",
            View = "/umbraco/views/propertyeditors/dropdown/dropdown.html",
            Options = new[]
            {
                new DataSourceParameterOption {Key = "sortorder", Name = "Sort order"},
                new DataSourceParameterOption {Key = "dateasc", Name = "Date"},
                new DataSourceParameterOption {Key = "datedesc", Name = "Date descending"},
            }
        };

        public static readonly Dictionary<string, Func<IEnumerable<IListableContent>, IEnumerable<IListableContent>>> sorters = new Dictionary<string, Func<IEnumerable<IListableContent>, IEnumerable<IListableContent>>>
        {
            {"sortorder", list => list.OrderBy(c => c.SortOrder)},
            {"dateasc", list => list.OrderBy(ContentDate)},
            {"datedesc", list => list.OrderByDescending(ContentDate)},
        };

        public static IEnumerable<IListableContent> Apply(IEnumerable<IListableContent> listables, IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("sort") && sorters.ContainsKey(parameters["sort"]))
            {
                listables = sorters[parameters["sort"]](listables);
            }
            return listables;
        }

        public static DateTime ContentDate(IListableContent c)
        {
            var publicationMeta = c as IPublicationMetadata;
            if (publicationMeta != null)
            {
                return publicationMeta.Date > DateTime.MinValue ? publicationMeta.Date : c.CreateDate;
            }
            return c.CreateDate;
        }
    }
}