using System;
using System.Web;
using NUnit.Framework;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.TestHelpers;

namespace Our.Umbraco.ContentList.Tests.DataSources.Listables
{
    [TestFixture]
    [Ignore("Sigh, why bother. X(")]
    public class ListablesByXPathDataSourceTests : BaseWebTest
    {
        [Test]
        public void Returns_All_Of_A_Doctype_Under_Root()
        {
            throw new NotImplementedException("Totally obsolete stuff");

            //SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());
            ////GetUmbracoContext("/", -1, null, true);
            //var content = Mock.Of<IPublishedContent>();
            //var datasource = new ListablesByXPathDataSource(
            //    new QueryParameters(
            //        content, 
            //        new Dictionary<string, string>
            //        {
            //            {"xpath", "//fakeListable"}
            //        }
            //    )
            //);
            //var result = datasource.Query(new PagingParameter());
            //Assert.AreEqual(3, result.Count());
        }

        // TODO: Orders
        // TODO: Counts

        //protected override void FreezeResolution()
        //{
        //    throw new NotImplementedException("Totally obsolete stuff");
        //    //var factoryResolver = (PublishedContentModelFactoryResolver)
        //    //    typeof(PublishedContentModelFactoryResolver)
        //    //        .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null)
        //    //        .Invoke(new object[0]);
        //    //PublishedContentModelFactoryResolver.Current = factoryResolver;
        //    //PublishedContentModelFactoryResolver.Current.SetFactory(new PublishedContentModelFactory(new []{typeof(FakeListable)}));

        //    base.FreezeResolution();
        //}

        protected override string GetXmlContent(int templateId)
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE root [ 
<!ELEMENT fakeListable ANY>
<!ATTLIST fakeListable id ID #REQUIRED>
<!ELEMENT site ANY>
<!ATTLIST site id ID #REQUIRED>
<!ELEMENT siteContent ANY>
<!ATTLIST siteContent id ID #REQUIRED>
<!ELEMENT resources ANY>
<!ATTLIST resources id ID #REQUIRED>

]>
<root id=""-1"">
  <site id=""3"" key=""8ff0f4d6-2f83-4122-84dc-d0a3f7e428e1"" parentID=""-1"" level=""1"" creatorID=""0"" sortOrder=""0"" createDate=""2015-12-21T15:37:33"" updateDate=""2016-01-05T12:54:05"" nodeName=""minsite.no"" urlName=""minsiteno"" path=""-1,1083"" isDoc="""" nodeType=""1072"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""0"" nodeTypeAlias=""site"">
    <footer></footer>
    <siteContent id=""2"" key=""70f4e704-3dd0-4ae4-ab09-03f7bdec94ea"" parentID=""1083"" level=""2"" creatorID=""0"" sortOrder=""0"" createDate=""2015-12-21T15:37:44"" updateDate=""2016-01-04T15:48:38"" nodeName=""Innhold"" urlName=""innhold"" path=""-1,1083,1084"" isDoc="""" nodeType=""1074"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""0"" nodeTypeAlias=""siteContent"">
      <fakeListable id=""1"" key=""f693d3ee-fe13-4245-b7ba-7a7c232351dd"" parentID=""1084"" level=""3"" creatorID=""0"" sortOrder=""1"" createDate=""2015-12-21T15:38:29"" updateDate=""2016-01-07T12:05:44"" nodeName=""Forsiden"" urlName=""forsiden"" path=""-1,1083,1084,1085"" isDoc="""" nodeType=""1082"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1053"" nodeTypeAlias=""page"">
      </fakeListable>
    </siteContent>
    <resources id=""4"" key=""70f4e704-3dd0-4ae4-ab09-03f7bdec94ea"" parentID=""1083"" level=""2"" creatorID=""0"" sortOrder=""0"" createDate=""2015-12-21T15:37:44"" updateDate=""2016-01-04T15:48:38"" nodeName=""Innhold"" urlName=""ressurser"" path=""-1,1083,1084"" isDoc="""" nodeType=""1074"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""0"" nodeTypeAlias=""siteContent"">
      <fakeListable id=""5"" key=""f693d3ee-fe13-4245-b7ba-7a7c232351dd"" parentID=""1084"" level=""3"" creatorID=""0"" sortOrder=""1"" createDate=""2015-12-21T15:38:29"" updateDate=""2016-01-07T12:05:44"" nodeName=""Forsiden"" urlName=""ressursside"" path=""-1,1083,1084,1085"" isDoc="""" nodeType=""1082"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1053"" nodeTypeAlias=""page"">
        <fakeListable id=""6"" key=""f693d3ee-fe13-4245-b7ba-7a7c232351dd"" parentID=""1084"" level=""3"" creatorID=""0"" sortOrder=""1"" createDate=""2015-12-21T15:38:29"" updateDate=""2016-01-07T12:05:44"" nodeName=""Forsiden"" urlName=""ressursbarn"" path=""-1,1083,1084,1085"" isDoc="""" nodeType=""1082"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1053"" nodeTypeAlias=""page"">
        </fakeListable>
      </fakeListable>
    </resources>
  </site>
</root>
";
        }
    }

    public class FakeListable : PublishedContentModel, IListableContent
    {
        public FakeListable(IPublishedContent content) : base(content)
        {
        }

        public IHtmlString ListHeading { get; set; }
        public string ListImageUrl { get; set; }
        public IHtmlString ListSummary { get; set; }
        public IHtmlString ReadMoreText { get; set; }
        public string DocumentTypeAlias { get; }
    }
}
