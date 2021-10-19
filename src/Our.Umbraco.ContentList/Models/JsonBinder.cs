using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class JsonBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var paramName = bindingContext.ModelName;
            var providedValue = bindingContext.ValueProvider.GetValue(paramName);
            if (providedValue.FirstValue != null &&
                providedValue.FirstValue.GetType() == bindingContext.ModelType)
            {
                bindingContext.Result = ModelBindingResult.Success(providedValue.FirstValue);
            }
            else
            {
                var requestBody = bindingContext.HttpContext.Request.Body;
                requestBody.Position = 0;
                var stream = requestBody;
                var readStream = new StreamReader(stream, Encoding.UTF8);
                var json = await readStream.ReadToEndAsync();
                bindingContext.Result = ModelBindingResult.Success(JsonConvert.DeserializeObject(json, bindingContext.ModelType));
            }
        }
    }
}