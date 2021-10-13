using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.ContentList.Models
{
    public interface IParameterValue
    {
        string Key { get; set; }
        object Value { get; set; }
    }
}
