using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Infrastructure.PublishedCache;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class ContentNodeSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<JObject>(reader);
            var contentNode = new ContentNode(
                obj.Value<int>("Id"),
                new Guid(obj.Value<string>("Uid")),
                obj.Value<int>("Level"),
                obj.Value<string>("Path"),
                obj.Value<int>("SortOrder"),
                obj.Value<int>("ParentContentId"),
                obj.Value<DateTime>("CreateDate"),
                obj.Value<int>("CreatorId")
            );
            return contentNode;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ContentNode);
        }
    }
}