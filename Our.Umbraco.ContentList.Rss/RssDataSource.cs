using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core;

namespace Our.Umbraco.ContentList.Rss
{
    [DataSourceMetadata(typeof(RssDataSourceMetaData))]
    public class RssDataSource : IListableDataSource, IDisposable
    {
        private readonly HttpClient client;
        private bool shouldDispose = false;
        private rss cachedFeed;

        public RssDataSource()
            : this(new HttpClient())
        {
            shouldDispose = true;
        }

        public RssDataSource(HttpClient client)
        {
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
            if (cachedFeed != null)
            {
                return cachedFeed;
            }
            var response = Task.Run(async () => await GetStream(query)).Result;
            var serializer = new XmlSerializer(typeof(rss));
            cachedFeed = (rss) serializer.Deserialize(response);
            return cachedFeed;
        }

        private async Task<Stream> GetStream(ContentListQuery query)
        {
            var url = query.CustomParameters["url"];
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
