using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Web
{
    [ModelBinder(typeof(JsonBinder))]
    public class ContentListParameters
    {
        [JsonProperty("datasource")]
        public ContentListDataSource DataSource { get; set; }
        [JsonProperty("view")]
        public string View { get; set; }
        [JsonProperty("pagesize")]
        public int PageSize { get; set; }
        [JsonProperty("showPaging")]
        public bool ShowPaging { get; set; }
        [JsonProperty("columns")]
        public ContentListColumns Columns { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("skip")]
        public int Skip { get; set; }
    }

    public class ContentListDataSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("parameters")]
        public List<DataSourceParameterValue> Parameters { get; set; }

    }

    public class ContentListColumns
    {
        [JsonProperty("small")]
        public int Small { get; set; }
        [JsonProperty("medium")]
        public int Medium { get; set; }
        [JsonProperty("large")]
        public int Large { get; set; }
    }

    public class DataSourceParameterValue : IParameterValue
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    [ModelBinder(typeof(JsonBinder))]
    public class PreviewContentListParameters : ContentListParameters
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
