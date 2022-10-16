using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Controllers;

public class LowerCaseJsonBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var paramName = bindingContext.ModelName;
        var providedValue = bindingContext.ValueProvider.GetValue(paramName);
        if (providedValue.FirstValue != null &&
            providedValue.FirstValue.GetType() == bindingContext.ModelType)
            return;

        var request = bindingContext.HttpContext.Request;

        var stream = request.Body;
        var readStream = new StreamReader(stream, Encoding.UTF8);
        var json = await readStream.ReadToEndAsync();
        bindingContext.Result = ModelBindingResult.Success(JsonConvert.DeserializeObject(json, bindingContext.ModelType));
    }
}