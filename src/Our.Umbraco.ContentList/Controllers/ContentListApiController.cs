﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Controllers
{
    [PluginController("OurContentList")]
    [LowerCaseJson]
    [JsonExceptionFilter]
    [IsBackOffice]
    [UmbracoUserTimeoutFilter]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [DisableBrowserCache]
    [UmbracoRequireHttps]
    [MiddlewareFilter(typeof(UnhandledExceptionLoggerFilter))]
    public class ContentListApiController : UmbracoApiController
    {
        private readonly string path;
        private readonly IServiceProvider serviceProvider;
        private readonly IRazorViewEngine viewEngine;

        public ContentListApiController(
            IServiceProvider serviceProvider, 
            IHostingEnvironment env,
            IRazorViewEngine viewEngine
        )
        {
            this.serviceProvider = serviceProvider;
            this.viewEngine = viewEngine;

            // TODO: Factory this...
            path = env.MapPathContentRoot("~/Views/Partials/ContentList");
        }

        [HttpGet]
        public IEnumerable<IDataSourceMetadata> GetDataSources()
        {
            return serviceProvider.GetServices<IDataSourceMetadata>();
        }

        [HttpGet]
        public IEnumerable<Object> ListTemplates()
        {
            return Enumerable.Empty<Object>();
        }

        [HttpGet]
        public List<ListTemplate> GetTemplates()
        {
            List<ListTemplate> templates = new List<ListTemplate>();
            var rootDir = new DirectoryInfo(path);

            if (rootDir.Exists)
            {
                templates.AddRange(
                    rootDir
                        .EnumerateDirectories()
                        .Where(p => System.IO.File.Exists(p.FullName + "/List.cshtml"))
                        .Select(MapTemplate)
                        .ToList()
                );
            }

            if (templates.Count == 0)
            {
                templates.Add(new ListTemplate("Sample")
                {
                    DisplayName = "Sample"
                });
            }

            return templates;
        }

        private ListTemplate MapTemplate(DirectoryInfo p)
        {
            var list = new ListTemplate(p.Name);
            var configPath = Path.Combine(p.FullName, "list.json");
            if (System.IO.File.Exists(configPath))
            {
                MapFromJsonConfig(configPath, list);
            }

            var viewPath = Path.Combine(p.FullName, "list.cshtml");
            var viewResult = viewEngine.GetView(null, viewPath, false);
            var metadata = ViewMetadata.GetMetadata(viewResult);
            if (metadata != ViewMetadata.Unknown)
            {
                list.DisplayName = metadata.Name;
                list.CompatibleSources = metadata.CompatibleDataSources.Select(x => x.GetFullNameWithAssembly()).ToArray();
            }

            return list;
        }

        private static void MapFromJsonConfig(string configPath, ListTemplate list)
        {
            try
            {
                var content = JsonConvert.DeserializeObject<JObject>(System.IO.File.ReadAllText(configPath));
                if (content?.GetValue("compatibleSources") != null)
                {
                    var sources = content.Value<JArray>("compatibleSources");
                    if (sources != null)
                    {
                        list.CompatibleSources = sources.ToObject<string[]>();
                    }
                }

                list.DisplayName = content?.Value<string>("displayName");
                list.DisableColumnsSetting = content?.Value<bool>("disableColumnsSetting") ?? false;
            }
            catch
            {
            }
        }
    }
}
