using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class PublishedContentContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Cast<MemberInfo>().ToList();
            if (objectType.Implements<IPublishedContent>())
            {
                var contentMembers = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => new[] {"Name", "UrlSegment", "SortOrder"}.Contains(x.Name));
                members.AddRange(contentMembers);
            }
            return members;
        }
    }
}