using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.ContentList.Web
{
    [ModelBinder(typeof(JsonBinder))]
    public class ContentListConfiguration
    {
        [JsonProperty("datasource")]
        public ContentListDataSource DataSource { get; set; } = new ContentListDataSource();
        [JsonProperty("view")]
        public string View { get; set; }
        [JsonProperty("pagesize")]
        public int PageSize { get; set; }
        [JsonProperty("showPaging")]
        public bool ShowPaging { get; set; }
        [JsonProperty("columns")]
        public ContentListColumns Columns { get; set; } = new ContentListColumns();

        [JsonProperty("skip")]
        public int Skip { get; set; }

        public string CreateHash()
        {
            var jobj = JObject.FromObject(this);
            jobj.Remove("page");
            var json = jobj.ToString();
            var hash = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(json))).Replace("=", "").Replace("+", "-");
            return hash;
        }
    }

    public class DataSourceParameterValue : IParameterValue
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    [ModelBinder(typeof(JsonBinder))]
    public class PreviewContentListConfiguration : ContentListConfiguration
    {
        [JsonProperty("contentId")]
        public int ContentId { get; set; }
    }

    public class JsonBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var paramName = bindingContext.ModelName;
            var providedValue = bindingContext.ValueProvider.GetValue(paramName);
            if (providedValue != null && providedValue.RawValue != null &&
                providedValue.RawValue.GetType() == bindingContext.ModelType)
                return providedValue.RawValue;

            controllerContext.HttpContext.Request.InputStream.Position = 0;
            var stream = controllerContext.HttpContext.Request.InputStream;
            var readStream = new StreamReader(stream, Encoding.UTF8);
            var json = readStream.ReadToEnd();
            return JsonConvert.DeserializeObject(json, bindingContext.ModelType);
        }
    }
}
