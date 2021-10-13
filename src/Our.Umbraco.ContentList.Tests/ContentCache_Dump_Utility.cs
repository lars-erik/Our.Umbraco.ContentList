using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Tests.Common.Testing;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class ContentCache_Dump_Utility
    {
        public ContentCache_Dump_Utility()
        {
        }

        [Test]
        [Explicit]
        public void Dump_All_Website_Content()
        {
            var contentSupport = new ContentCacheSupport();
            var contentKits = contentSupport.GetFromCacheFile();
            Console.WriteLine(JsonConvert.SerializeObject(contentKits, Formatting.Indented));
        }
    }
}
