using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.ContentList.Web
{
    [PluginController("OurContentList")]
    public class ContentListApiController : UmbracoAuthorizedApiController
    {
        private string path = HostingEnvironment.MapPath("~/Views/Partials/ContentList");
        private string samplePath = HostingEnvironment.MapPath("~/App_Plugins/Our.Umbraco.ContentList/Views/ContentList/ListViews");

        public ContentListApiController(IGlobalSettings globalSettings, IUmbracoContextAccessor umbracoContextAccessor, ISqlContext sqlContext, ServiceContext services, AppCaches appCaches, IProfilingLogger logger, IRuntimeState runtimeState, UmbracoHelper umbracoHelper)
            : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
        }

        public ContentListApiController(string path, string samplePath, IGlobalSettings globalSettings, IUmbracoContextAccessor umbracoContextAccessor, ISqlContext sqlContext, ServiceContext services, AppCaches appCaches, IProfilingLogger logger, IRuntimeState runtimeState, UmbracoHelper umbracoHelper)
            : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            this.path = path;
            this.samplePath = samplePath;
        }

        [HttpGet]
        public List<ListTemplate> ListTemplates()
        {
            List<ListTemplate> templates = new List<ListTemplate>();
            var rootDir = new DirectoryInfo(path);

            if (rootDir.Exists)
            { 
                templates.AddRange(
                    rootDir
                    .EnumerateDirectories()
                    .Where(p => File.Exists(p.FullName + "/List.cshtml"))
                    .Select(MapTemplate)
                    .ToList()
                );
            }
            
            var sampleInfo = new DirectoryInfo(samplePath);
            if (sampleInfo.Exists)
            {
                var legacyTemplates = sampleInfo
                    .EnumerateFiles("*.cshtml")
                    .Select(f => new ListTemplate(f.Name.Replace(".cshtml", "")))
                    .Where(n => templates.All(x => x.Name != n.Name))
                    .ToList();

                if (legacyTemplates.Count > 1 || templates.Count > 0)
                {
                    var sample = legacyTemplates.FirstOrDefault(x => x.Name == "Sample");
                    if (sample != null)
                    {
                        legacyTemplates.Remove(sample);
                    }
                }
                
                templates.AddRange(legacyTemplates);
            }

            return templates;
        }

        private static ListTemplate MapTemplate(DirectoryInfo p)
        {
            var list = new ListTemplate(p.Name);
            var configPath = Path.Combine(p.FullName, "list.json");
            if (File.Exists(configPath))
            {
                try
                {
                    var content = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(configPath));
                    if (content?.GetValue("compatibleSources") != null)
                    {
                        var sources = content.Value<JArray>("compatibleSources");
                        if (sources != null)
                        {
                            list.CompatibleSources = sources.ToObject<string[]>();
                        }
                    }

                    list.DisableColumnsSetting = content?.Value<bool>("disableColumnsSetting") ?? false;
                }
                catch
                {
                }
            }
            return list;
        }
    }
}
