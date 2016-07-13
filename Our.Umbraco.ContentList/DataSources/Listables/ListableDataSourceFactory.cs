using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Our.Umbraco.ContentList.DataSources.Listables
{
    public class ListableDataSourceFactory
    {
        static readonly object LockObj = new object();
        private static List<DataSourceMetadata> datasources;

        public virtual IListableDataSource Create(string key, QueryParameters queryParameters)
        {
            var type = FindDataSourceType(key);

            return (IListableDataSource) Activator.CreateInstance(type, (object) queryParameters);
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
            lock(LockObj)
            {
                if (datasources != null)
                    return datasources;

                var listableDataSourceType = typeof (IListableDataSource);
                var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(TryGetTypes);
                var listableDataSourceTypes = allTypes
                    .Where(listableDataSourceType.IsAssignableFrom)
                    .Where(t => !t.IsAbstract && !t.IsInterface);
                datasources = listableDataSourceTypes.Select(FindMetadata).ToList();
                return datasources;
            }
        }

        private static IEnumerable<Type> TryGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch
            {
                return new Type[0];
            }
        }

        private static DataSourceMetadata FindMetadata(Type type)
        {
            var metadataTypes = type.GetCustomAttributes<DataSourceMetadataAttribute>().ToList();
            DataSourceMetadata metadata = null;
            if (metadataTypes.Any())
            {
                var metadataType = metadataTypes[0].MetadataType;
                metadata = (DataSourceMetadata) Activator.CreateInstance(metadataType);
            }
            else
            {
                metadata = new SimpleDataSourceMetadata(type);
            }
            return metadata;
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
