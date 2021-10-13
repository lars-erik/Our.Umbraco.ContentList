using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Infrastructure.PublishedCache.DataSource;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class ContentCacheSupport
    {
        public BPlusTree<int, ContentNodeKit> GetFromCacheFile()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDirectory = Path.Combine(baseDirectory, @"..\..\..\..");
            var cacheFilePath = Path.Combine(solutionDirectory, @"Our.Umbraco.ContentList.Web\umbraco\Data\TEMP\NuCache\NuCache.Content.db");
            return GetFromCacheFile(cacheFilePath);
        }

        public BPlusTree<int, ContentNodeKit> GetFromCacheFile(string path)
        {
            var allContent = BTree.GetTree(path, true, new NuCacheSettings());
            return allContent;
        }
    }
}
