using System.IO;

namespace Our.Umbraco.ContentList.Rss.Tests
{
    public class RssTestData
    {
        public static Stream GetRssStream()
        {
            return typeof(RssTestData).Assembly
                .GetManifestResourceStream("Our.Umbraco.ContentList.Rss.Tests.TestData.Blog.rss.xml");
        }
    }
}