using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public static class JsonExtensions
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new TypeOnlyMembersContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}