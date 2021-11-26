using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class IntegrationModes : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var dataSource = new TestFixtureData("dataSource");
            dataSource.Properties.Add(PropertyNames.Category, "Unit");
            yield return dataSource;

            var component = new TestFixtureData("component");
            component.Properties.Add(PropertyNames.Category, "Integrated");
            yield return component;
        }
    }
}