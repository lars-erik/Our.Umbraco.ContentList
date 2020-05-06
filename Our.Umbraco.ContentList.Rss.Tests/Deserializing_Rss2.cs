using System.Xml.Serialization;
using NUnit.Framework;

namespace Our.Umbraco.ContentList.Rss.Tests
{
    [TestFixture]
    public class Deserializing_Rss2
    {
        [Test]
        public void Returns_Full_Graph()
        {
            var all = GetType().Assembly.GetManifestResourceNames();
            using (var stream = RssTestData.GetRssStream())
            {
                Assert.IsNotNull(stream);
                
                var serializer = new XmlSerializer(typeof(rss));
                var feed = (rss)serializer.Deserialize(stream);

                Assert.That(feed.channel.item, Has.Length.EqualTo(25));
            }
        }
    }
}
