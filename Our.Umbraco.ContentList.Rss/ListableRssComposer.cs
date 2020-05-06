using Our.Umbraco.ContentList.Install;
using Umbraco.Core.Composing;

namespace Our.Umbraco.ContentList.Rss
{
    [ComposeAfter(typeof(ListableDataSourcesComposer))]
    public class ListableRssComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.WithCollectionBuilder<ListableDataSourceCollectionBuilder>()
                .Add<RssDataSource>();
        }
    }
}
