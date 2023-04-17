using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Models
{
    public class ViewMetadata
    {
        public static readonly ViewMetadata Unknown = new ViewMetadata
        (
            name: "Unknown",
            description: "No metadata available",
            compatibleDataSources: Array.Empty<Type>(),
            incompatibleDataSources: Array.Empty<Type>()
        );

        public string Name { get; }
        public string Description { get; }
        public Type[] CompatibleDataSources { get; }
        public Type[] IncompatibleDataSources { get; }
        public DataSourceParameterDefinition[] Parameters { get; } 

        public ViewMetadata(
            string name, 
            string description = "", 
            Type[] compatibleDataSources = null, 
            Type[] incompatibleDataSources = null,
            DataSourceParameterDefinition[] parameters = null)
        {
            Name = name;
            Description = description;
            CompatibleDataSources = compatibleDataSources ?? Array.Empty<Type>();
            IncompatibleDataSources = incompatibleDataSources ?? Array.Empty<Type>();
            Parameters = parameters ?? Array.Empty<DataSourceParameterDefinition>();
        }

        public static ViewMetadata GetMetadata(ViewEngineResult viewResult)
        {
            return GetMetadata(viewResult?.View);
        }

        private static ViewMetadata GetMetadata(IView view)
        {
            var razorView = (RazorView)view;
            var type = razorView?.RazorPage.GetType();
            if (type == null) return Unknown;

            var metadataProp = type?.GetProperty("Metadata", BindingFlags.Public | BindingFlags.Static);
            var value = metadataProp?.GetValue(null) as ViewMetadata;
            if (value == null)
            {
                var shortStringHelper = new DefaultShortStringHelper(new DefaultShortStringHelperConfig());
                value = new ViewMetadata(
                    name: shortStringHelper.SplitPascalCasing(Directory.GetParent(view.Path)?.Name ?? "Invalid Path", ' '),
                    description: "",
                    compatibleDataSources: Array.Empty<Type>(),
                    incompatibleDataSources: Array.Empty<Type>(),
                    parameters: Array.Empty<DataSourceParameterDefinition>()
                );
            }
            return value;
        }
    }
}
