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
    [ModelBinder(typeof(JsonBinder))]
    public class ContentListConfiguration
    {
        [JsonProperty("datasource")]
        public ContentListDataSource DataSource { get; set; } = new ContentListDataSource();
        [JsonProperty("view")]
        public string View { get; set; }
        [JsonProperty("pagesize")]
        public int PageSize { get; set; }
        [JsonProperty("showPaging")]
        public bool ShowPaging { get; set; }
        [JsonProperty("columns")]
        public ContentListColumns Columns { get; set; } = new ContentListColumns();

        [JsonProperty("skip")]
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
