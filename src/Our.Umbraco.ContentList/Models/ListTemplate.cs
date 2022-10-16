using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.ContentList.Models
{
    public class ListTemplate
    {
        public ListTemplate(string name)
        {
            Name = name;
            CompatibleSources = new string[0];
        }

        public string Name { get; set; }
 
        public string[] CompatibleSources { get; set; }

        public bool DisableColumnsSetting { get; set; }

        public string DisplayName { get; set; }
        public bool Compiles { get; set; }
    }
}
