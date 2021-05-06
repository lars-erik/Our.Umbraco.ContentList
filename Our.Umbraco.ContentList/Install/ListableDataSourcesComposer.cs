using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core.Composing;

namespace Our.Umbraco.ContentList.Install
{
    public class ListableDataSourcesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var allTypes = composition.TypeLoader.GetTypes<IListableDataSource>(specificAssemblies:new []{ GetType().Assembly });
            composition
                .WithCollectionBuilder<ListableDataSourceCollectionBuilder>()
                .Add(allTypes);
        }
    }

    public static class CompositionExtensions
    {
        public static void WithListableDataSources(this Composition composition)
        {
            new ListableDataSourcesComposer().Compose(composition);
        }
    }

    public class ListableDataSourceCollectionBuilder : SetCollectionBuilderBase<ListableDataSourceCollectionBuilder, ListableDataSourceCollection, IListableDataSource>
    {
        protected override ListableDataSourceCollectionBuilder This => this;

        protected override Lifetime CollectionLifetime => Lifetime.Transient;
    }

    public class ListableDataSourceCollection : BuilderCollectionBase<IListableDataSource>
    {
        public ListableDataSourceCollection(IEnumerable<IListableDataSource> items) : base(items)
        {
        }
    }
}
