using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.ContentList.Models
{
    public class ContentListConfiguration
    {
        public ContentListDataSource DataSource { get; set; } = new ContentListDataSource();
        public string View { get; set; }
        public int PageSize { get; set; }
        public bool ShowPaging { get; set; }
        public ContentListColumns Columns { get; set; } = new ContentListColumns();

        public int Skip { get; set; }

        public string CreateHash()
        {
            var jobj = JObject.FromObject(this);
            jobj.Remove("page");
            var json = jobj.ToString();
            var hash = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(json))).Replace("=", "").Replace("+", "-");
            return hash;
        }
    }
}
