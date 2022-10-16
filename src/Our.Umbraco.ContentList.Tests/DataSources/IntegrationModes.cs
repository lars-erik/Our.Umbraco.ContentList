using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class IntegrationModes : IEnumerable
    {
        public const string DataSource = "dataSource";
        public const string Component = "component";
        private string[] modes;

        public IntegrationModes()
        {
            var modesString = (Environment.GetEnvironmentVariable("IntegrationModes", EnvironmentVariableTarget.Process) ?? "dataSource;component");
            modes = modesString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerator GetEnumerator()
        {
            if (modes.Contains(DataSource))
            {
                var dataSource = new TestFixtureData(DataSource);
                dataSource.Properties.Add(PropertyNames.Category, "Unit");
                yield return dataSource;
            }

            if (modes.Contains(Component))
            {
                var component = new TestFixtureData(Component);
                component.Properties.Add(PropertyNames.Category, "Integrated");
                yield return component;
            }
        }
    }
}