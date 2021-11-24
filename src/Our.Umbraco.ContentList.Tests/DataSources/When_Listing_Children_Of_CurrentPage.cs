using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApprovalTests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;
using Our.Umbraco.ContentList.Web.Models;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    [TestFixture]
    public class When_Listing_Children_Of_CurrentPage
    {
        private UmbracoSupport support;

        [SetUp]
        public void Setup()
        {
            var cacheSupport = new ContentCacheSupport();
            var tree = cacheSupport.GetFromJsonResource(GetType().FullName);
            support = new UmbracoSupport(tree, Tests.Setup.ContentTypes);
            support.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            support.TearDownUmbraco();
        }

        [Test]
        public void All_Published_Children_Are_Listed()
        {
            var ctx = support.GetUmbracoContext();
            var home = ctx.Content.GetById(1000);

            var dataSource = new ListableChildrenDataSource();
            var result = dataSource.Query(
                new ContentListQuery
                {
                    ContextContent = home
                },
                new QueryPaging(10)
            );

            Assert.That(result.ToList(), Has.Count.EqualTo(2));
            Assert.That(result.First(), Is.TypeOf<Page>());

            Approvals.VerifyJson(ToJson(result));

        }

        [Test]
        public void Gets_Right_Metadata()
        {
            Assert.Inconclusive("Not done yet. Must move to DI.");
        }

        private static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new TypeOnlyMembersContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }

    public class TypeOnlyMembersContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Cast<MemberInfo>().ToList();
        }
    }
}
