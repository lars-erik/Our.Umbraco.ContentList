using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.DataSources;
using VerifyNUnit;

namespace Our.Umbraco.ContentList.Tests.Views
{
    [TestFixture]
    public class View_Metadata : DataSourceTestBase
    {
        public View_Metadata() : base(IntegrationModes.Component)
        {
        }

        [Test]
        public async Task Are_Read_From_Static_Metadata_Property()
        {
            var engine = Support.Services.GetService<IRazorViewEngine>();
            var path = "~/Views/Partials/ContentList/SimpleTheme/List.cshtml";
            var foundView = engine.GetView(null, path, false);
            var value = ViewMetadata.GetMetadata(foundView);
            await Verifier.Verify(value);
        }

        [Test]
        public async Task Defaults_To_Info_About_Type()
        {
            var engine = Support.Services.GetService<IRazorViewEngine>();
            var path = "~/Views/Partials/ContentList/NoMetadata/List.cshtml";
            var foundView = engine.GetView(null, path, false);
            var value = ViewMetadata.GetMetadata(foundView);
            await Verifier.Verify(value);
        }

        [Test]
        public async Task Returns_Unknown_For_Missing_Views()
        {
            var engine = Support.Services.GetService<IRazorViewEngine>();
            var path = "~/Views/Partials/ContentList/NonExisting/List.cshtml";
            var foundView = engine.GetView(null, path, false);
            var value = ViewMetadata.GetMetadata(foundView);
            Assert.AreSame(ViewMetadata.Unknown, value);
            await Verifier.Verify(value);
        }
    }
}
