using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Components
{
    [ViewComponent(Name="ContentListPager")]
    public class PagerComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ContentListPaging paging)
        {
            return View("Pager", paging);
        }
    }
}
