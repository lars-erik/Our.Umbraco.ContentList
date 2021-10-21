using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.ContentList.DataSources;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.ContentList.Composition
{
    public class DataSourceMetadataCollectionBuilder : SetCollectionBuilderBase<DataSourceMetadataCollectionBuilder, DataSourceMetadataCollection, IDataSourceMetadata>
    {
        protected override DataSourceMetadataCollectionBuilder This => this;

        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;

        public override void RegisterWith(IServiceCollection services)
        {
            base.RegisterWith(services);

            foreach (var type in GetTypes())
            {
                services.AddTransient(typeof(IDataSourceMetadata), type);

                var prop = type.GetProperty("For", BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                var sourceType = (Type)prop?.GetValue(null);
                if (sourceType != null)
                {
                    services.AddTransient(typeof(IListableDataSource), sourceType);
                }
            }
        }
    }

    public class DataSourceMetadataCollection : BuilderCollectionBase<IDataSourceMetadata>
    {
        public DataSourceMetadataCollection() : base(() => new IDataSourceMetadata[0])
        {
        }
    }

    public static class CollectionBuilderExtensions
    {
        public static DataSourceMetadataCollectionBuilder WithListableDataSources(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<DataSourceMetadataCollectionBuilder>();
    }
}
