using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Composing;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    public class ListableDataSourceFactory
    {
        static readonly object LockObj = new object();

        public virtual IListableDataSource Create(string key)
        {
            var type = FindDataSourceType(key);
            return (IListableDataSource) Current.Factory.GetInstance(type);
        }

        public static IList<DataSourceParameterDefinition> CreateParameters(string typeName)
        {
            var type = FindDataSourceType(typeName);
            return CreateParameters(type);
        }

        public static IList<DataSourceParameterDefinition> CreateParameters(Type type)
        {
            var metadata = FindMetadata(type);
            if (metadata != null)
                return metadata.Parameters;
            return new DataSourceParameterDefinition[0];
        }

        public static List<DataSourceMetadata> GetDataSources()
        {
            // TODO: Should we be able to access IRegister?
            var listableDataSourceTypes = Current.Factory
                .GetAllInstances<IListableDataSource>()
                .Select(x => x.GetType());
            var dataSources = listableDataSourceTypes.Select(FindMetadata).ToList();
            return dataSources;
        }

        private static DataSourceMetadata FindMetadata(Type type)
        {
            var metadataTypes = type.GetCustomAttributes<DataSourceMetadataAttribute>().ToList();
            if (metadataTypes.Any())
            {
                var metadataType = metadataTypes[0].MetadataType;
                return (DataSourceMetadata) Activator.CreateInstance(metadataType);
            }

            return new SimpleDataSourceMetadata(type);
        }

        private static Type FindDataSourceType(string key)
        {
            var type = Type.GetType(key);
            if (type == null)
                throw new ArgumentException(String.Format("Couldn't find listable datasource '{0}'", key));
            if (type.GetInterface("IListableDataSource") == null)
                throw new InvalidCastException(String.Format("Type '{0}' is not a valid IListableDataSource", key));
            return type;
        }
    }
}
