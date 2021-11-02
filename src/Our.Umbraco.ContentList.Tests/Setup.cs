using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests.Reporters;
using NUnit.Framework;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;
using Umbraco.Cms.Tests.Common.Testing;

[assembly:UseReporter(typeof(VisualStudioReporter))]

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

        public static void ContentTypes(IDataTypeService dataTypeService, IContentTypeService contentTypeService)
        {
            var dataType = new DataTypeBuilder()
                .WithId(1)
                .WithDatabaseType(ValueStorageType.Nvarchar)
                .WithName("List heading")
                .Build();

            dataTypeService.Save(dataType);

            var contentTypeBuilder = new ContentTypeBuilder();

            var listableContentBuilder = contentTypeBuilder
                .WithAlias("listableContent")
                .WithId(1058);
            listableContentBuilder
                .AddPropertyType()
                .WithAlias("listHeading")
                .WithDataTypeId(1)
                .Build();
            var listableContentType = listableContentBuilder.Build();

            var pageBuilder = contentTypeBuilder
                .WithAlias("page")
                .WithId(1057);
            pageBuilder
                .WithParentContentType(listableContentType);
            pageBuilder
                .AddAllowedContentType()
                .WithAlias("page");

            var pageType = pageBuilder.Build();

            contentTypeService.Save(listableContentType);
            contentTypeService.Save(pageType);
        }
    }
}
