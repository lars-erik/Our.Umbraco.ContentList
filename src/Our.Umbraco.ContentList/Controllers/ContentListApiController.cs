using System;
using System.Buffers;
using System.Collections.Generic;
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
using Newtonsoft.Json.Serialization;
using Our.Umbraco.ContentList.DataSources;
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
        private readonly IServiceProvider serviceProvider;

        public ContentListApiController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
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
