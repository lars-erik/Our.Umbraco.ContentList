using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
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

        public ContentListApiController()
        {
        }

        public ContentListApiController(UmbracoContext umbracoContext)
            : base(umbracoContext)
        {
        }

        public ContentListApiController(UmbracoContext umbracoContext, string path, string samplePath)
            : base(umbracoContext)
        {
            this.path = path;
            this.samplePath = samplePath;
        }

        [HttpGet]
        public List<string> ListTemplates()
        {
            List<string> templates = new List<string>();
            var rootInfo = new DirectoryInfo(path);

            if (rootInfo.Exists)
            { 
                templates.AddRange(
                    rootInfo
                    .EnumerateDirectories()
                    .Where(p => File.Exists(p.FullName + "/List.cshtml"))
                    .Select(p => p.Name)
                    .ToList()
                );
            }
            
            var sampleInfo = new DirectoryInfo(samplePath);
            if (sampleInfo.Exists)
            {
                var legacyTemplates = sampleInfo
                    .EnumerateFiles("*.cshtml")
                    .Select(f => f.Name.Replace(".cshtml", ""))
                    .Where(n => !templates.Contains(n))
                    .ToList();

                if (legacyTemplates.Count > 1 || templates.Count > 0)
                    legacyTemplates.Remove("Sample");
                
                templates.AddRange(legacyTemplates);
            }

            return templates;
        } 
    }
}
