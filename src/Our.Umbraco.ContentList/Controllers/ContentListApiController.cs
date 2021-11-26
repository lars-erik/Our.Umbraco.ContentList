using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.Formatters;

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

        public ContentListApiController(IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            this.serviceProvider = serviceProvider;

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

        private static ListTemplate MapTemplate(DirectoryInfo p)
        {
            var list = new ListTemplate(p.Name);
            var configPath = Path.Combine(p.FullName, "list.json");
            if (System.IO.File.Exists(configPath))
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
            return list;
        }
    }

    public class LowerCaseJsonAttribute : TypeFilterAttribute
    {
        public LowerCaseJsonAttribute() : base(typeof(AngularJsonOnlyConfigurationFilter))
        {
            Order = 1; // Must be low, to be overridden by other custom formatters, but higher then all framework stuff.
        }

        private class AngularJsonOnlyConfigurationFilter : IResultFilter
        {
            private readonly ArrayPool<char> _arrayPool;
            private readonly IOptions<MvcOptions> _options;

            public AngularJsonOnlyConfigurationFilter(ArrayPool<char> arrayPool, IOptions<MvcOptions> options)
            {
                _arrayPool = arrayPool;
                _options = options;
            }

            public void OnResultExecuted(ResultExecutedContext context)
            {
            }

            public void OnResultExecuting(ResultExecutingContext context)
            {
                if (context.Result is ObjectResult objectResult)
                {
                    var serializerSettings = new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Converters = { new VersionConverter() }
                    };

                    objectResult.Formatters.Clear();
                    objectResult.Formatters.Add(new AngularJsonMediaTypeFormatter(serializerSettings, _arrayPool, _options.Value));
                }
            }
        }
    }
}
