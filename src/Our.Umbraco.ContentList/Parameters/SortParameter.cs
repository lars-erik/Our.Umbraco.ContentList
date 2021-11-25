using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Parameters
{
    public static class ListableSorting
    {
        public static readonly DataSourceParameterDefinition Parameter = new DataSourceParameterDefinition
        {
            Key = "sort",
            Label = "Sort by",
            View = "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/dropdown/dropdown.html",
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

        public static readonly Dictionary<string, Func<IEnumerable<IListableContent>, IEnumerable<IListableContent>>> sorters = new Dictionary<string, Func<IEnumerable<IListableContent>, IEnumerable<IListableContent>>>
        {
            {"sortorder", list => list.OrderBy(c => c.SortOrder)},
            {"dateasc", list => list.OrderBy(ContentDate)},
            {"datedesc", list => list.OrderByDescending(ContentDate)},
        };

        public static IEnumerable<IListableContent> ApplySorting(this IEnumerable<IListableContent> listables, IDictionary<string, object> parameters)
        {
            if (parameters.ContainsKey("sort") && sorters.ContainsKey(parameters["sort"]?.ToString() ?? ""))
            {
                listables = sorters[parameters["sort"].ToString()](listables);
            }
            return listables;
        }

        public static DateTime ContentDate(IListableContent c)
        {
            //var publicationMeta = c as IPublicationMetadata;
            //if (publicationMeta != null)
            //{
            //    return publicationMeta.Date > DateTime.MinValue ? publicationMeta.Date : c.SortDate;
            //}
            return c.SortDate;
        }
    }
}
