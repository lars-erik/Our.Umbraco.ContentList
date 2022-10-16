using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public static class JsonExtensions
    {
        public static string ToJson(this object data, bool indent = false)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PublishedContentContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            if (indent)
            {
                settings.Formatting = Formatting.Indented;
            }
            return JsonConvert.SerializeObject(data, settings);
        }
    }
}