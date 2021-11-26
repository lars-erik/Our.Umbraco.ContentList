using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public static class JsonExtensions
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new PublishedContentContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}