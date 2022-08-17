using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class IntegrationModes : IEnumerable
    {
        private string[] modes;

        public IntegrationModes()
        {
            var modesString = (Environment.GetEnvironmentVariable("IntegrationModes", EnvironmentVariableTarget.Process) ?? "dataSource;component");
            modes = modesString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerator GetEnumerator()
        {
            if (modes.Contains("dataSource"))
            {
                var dataSource = new TestFixtureData("dataSource");
                dataSource.Properties.Add(PropertyNames.Category, "Unit");
                yield return dataSource;
            }

            if (modes.Contains("component"))
            {
                var component = new TestFixtureData("component");
                component.Properties.Add(PropertyNames.Category, "Integrated");
                yield return component;
            }
        }
    }
}