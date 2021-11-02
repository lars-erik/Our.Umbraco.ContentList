﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.ContentList.Parameters
{
    public class LanguageParameter
    {
        public static void Add(List<DataSourceParameterDefinition> parameters, ILocalizationService localizationService)
        {
            var definition = Create(localizationService);

            if (definition != null)
            {
                parameters.Add(definition);
            }
        }

        public static DataSourceParameterDefinition Create(ILocalizationService localizationService)
        {
            var languages = localizationService.GetAllLanguages();
            DataSourceParameterDefinition definition = null;
            if (languages.Count() > 1)
            {
                definition = new DataSourceParameterDefinition
                {
                    Key = "language",
                    Label = "Language",
                    View = "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/dropdown/dropdown.html",
                    Config = new DataSourceConfig
                    {
                        Items = languages.Select(x => new DataSourceParameterOption {Id = x.IsoCode, Value = x.CultureName})
                            .ToList()
                    }
                };
            }

            return definition;
        }

        public static string Culture(ContentListQuery query)
        {
            string culture = null;
            if (query.CustomParameters.ContainsKey("language"))
            {
                culture = query.CustomParameters["language"] as string;
            }

            return culture;
        }
    }
}
