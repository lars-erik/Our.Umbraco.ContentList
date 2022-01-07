using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Our.Umbraco.ContentList.Rss
{
    [DataSourceMetadata(typeof(RssDataSourceMetaData))]
    public class RssDataSource : IListableDataSource, IDisposable
    {
        private readonly ILogger logger;
        private readonly AppCaches caches;
        private readonly HttpClient client;
        private bool shouldDispose = false;
        private rss cachedFeed;

        public RssDataSource(AppCaches caches, ILogger logger)
            : this(caches, logger, new HttpClient())
        {
            shouldDispose = true;
        }

        public RssDataSource(AppCaches caches, ILogger logger, HttpClient client)
        {
            this.logger = logger;
            this.caches = caches;
            this.client = client;
        }

        public IQueryable<IListableContent> Query(ContentListQuery query, QueryPaging paging)
        {
            var feed = GetFeed(query);
            return feed.channel.item
                .Skip((int)paging.PreSkip)
                .Skip((int)paging.Skip)
                .Take((int)paging.Take)
                .Cast<IListableContent>()
                .AsQueryable();
        }

        public long Count(ContentListQuery query, long preSkip)
        {
            var feed = GetFeed(query);
            return feed.channel.item.Length;
        }

        private rss GetFeed(ContentListQuery query)
        {
            var url = query.CustomParameter<string>("url");
            if (cachedFeed == null)
            {
                cachedFeed = caches.RuntimeCache.Get(url) as rss;
                logger.Debug<RssDataSource>($"RSS Cache lookup for {url} yielded {(cachedFeed == null ? "nothing" : "stuff")}");
            }

            if (cachedFeed != null)
            {
                return cachedFeed;
            }

            logger.Debug<RssDataSource>($"No cache for {url}, so fetching new feed");
            var response = Task.Run(async () => await GetStream(query)).Result;
            var serializer = new XmlSerializer(typeof(rss));
            cachedFeed = (rss) serializer.Deserialize(XmlReader.Create(response), new XmlDeserializationEvents
            {
                OnUnknownElement = AddUnknown
            });

            caches.RuntimeCache.Insert(url, () => cachedFeed, TimeSpan.FromMinutes(10));

            return cachedFeed;
        }

        private void AddUnknown(object sender, XmlElementEventArgs e)
        {
            if (e.ObjectBeingDeserialized is item item)
            {
                if (item.ExtraData.ContainsKey(e.Element.LocalName))
                {
                    item.ExtraData[e.Element.LocalName] += "; " + e.Element.InnerText;
                }
                else
                {
                    item.ExtraData.Add(e.Element.LocalName, e.Element.InnerText);
                }
            }
        }

        private async Task<Stream> GetStream(ContentListQuery query)
        {
            var url = query.CustomParameter<string>("url");
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStreamAsync();
        }

        public void Dispose()
        {
            if (shouldDispose)
            {
                client?.Dispose();
            }
        }
    }

    public class RssDataSourceMetaData : DataSourceMetadata
    {
        public RssDataSourceMetaData()
        {
            Key = typeof(RssDataSource).GetFullNameWithAssembly();
            Name = "RSS Feed";
            Parameters = new List<DataSourceParameterDefinition>
            {
                new DataSourceParameterDefinition
                {
                    Key = "url",
                    Label = "Url",
                    View = "/Umbraco/views/propertyeditors/textbox/textbox.html"
                }
            };
        }
    }
}
