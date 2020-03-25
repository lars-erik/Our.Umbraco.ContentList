using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.DataSources.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.TestHelpers;

namespace Our.Umbraco.ContentList.Tests.DataSources.PublishedContent
{
    [TestFixture]
    [Ignore("Won't bother for now. X('")]
    public class PublishedContentByXPathDataSourceTests : BaseWebTest
    {
        [Test]
        public void Returns_All_Of_A_Doctype_Under_Root()
        {
            throw new NotImplementedException("Obsolete stuff");
            //SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());
            ////GetUmbracoContext("/", -1, null, true);
            //var content = Mock.Of<IPublishedContent>();
            //var datasource = new PublishedContentByXPathDataSource(
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
}

