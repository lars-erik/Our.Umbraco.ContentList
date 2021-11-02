using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class When_Rendering_Pager
    {
        private TestServerFactory serverFactory;

        [Test]
        public async Task On_First_Page_Renders_One_Through_Five()
        {
            serverFactory = new TestServerFactory();
            var viewRenderer = serverFactory.GetRequiredService<ViewRenderer>();

            var pagerModel = new ContentListPaging
            {
                Total = 100,
                From = 0,
                To = 9,
                Page = 1,
                PageSize = 10,
                ShowPaging = true
            };
            
            var result = await viewRenderer.Render("PagerTest", pagerModel);
            
            Console.WriteLine(result);
            Approvals.VerifyHtml(result);
        }
    }
}
