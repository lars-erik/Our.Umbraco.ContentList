using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class When_Listing_Children
    {
        private UmbracoSupport support;

        [SetUp]
        public void Setup()
        {
            var contentSupport = new ContentCacheSupport();
            var contentKits = contentSupport.GetFromCacheFile();

            using var resource = GetType().Assembly.GetManifestResourceStream("Our.Umbraco.ContentList.Tests.site.json");
            using var reader = new StreamReader(resource);
            var json = reader.ReadToEnd();
            var content = JsonConvert.DeserializeObject<Dictionary<int, ContentNodeKit>>(
                json,
                new ContentNodeSerializer()
                );
            var tree = new BPlusTree<int, ContentNodeKit>();
            tree.AddRange(content);

            support = new UmbracoSupport(tree);
            support.Setup();

            SetupContentTypes();

            support.SetupContentCache();

        }

        private void SetupContentTypes()
        {
            var dataTypeService = support.GetService<IDataTypeService>();
            var contentTypeService = support.GetService<IContentTypeService>();

            var dataType = new DataTypeBuilder()
                .WithId(1)
                .WithDatabaseType(ValueStorageType.Nvarchar)
                .WithName("List heading")
                .Build();

            dataTypeService.Save(dataType);

            var contentTypeBuilder = new ContentTypeBuilder();

            var listableContentBuilder = contentTypeBuilder
                .WithAlias("listableContent")
                .WithId(1058);
            listableContentBuilder
                .AddPropertyType()
                .WithAlias("listHeading")
                .WithDataTypeId(1)
                .Build();
            var listableContentType = listableContentBuilder.Build();

            var pageBuilder = contentTypeBuilder
                .WithAlias("page")
                .WithId(1057);
            //pageBuilder
            //    .WithParentContentType(listableContentType);
            pageBuilder
                .AddAllowedContentType()
                .WithAlias("page");

            var pageType = pageBuilder.Build();

            contentTypeService.Save(listableContentType);
            contentTypeService.Save(pageType);
        }

        [TearDown]
        public void TearDown()
        {
            support.TearDownUmbraco();
        }

        [Test]
        public void Lists_Children_Of_Requested_Page()
        {
            var ctx = support.GetUmbracoContext();

            var home = ctx.Content.GetById(new Guid("64d01018-3dce-4efb-809f-a9705e166bbd"));

            Console.WriteLine(JsonConvert.SerializeObject(home, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            }));

            Assert.That(home.Children.ToList(), Has.Count.EqualTo(2));
        }
    }

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
