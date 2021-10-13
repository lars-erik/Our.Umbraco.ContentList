using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.Cms.Tests.Common.Testing;

namespace Our.Umbraco.ContentList.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public static void BeforeAll()
        {
            TestOptionAttributeBase.ScanAssemblies.Add(typeof(Setup).Assembly);
        }
    }
}
