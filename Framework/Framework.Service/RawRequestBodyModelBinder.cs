namespace ZTR.Framework.Service
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class RawRequestBodyModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;

            using var reader = new StreamReader(request.Body);
            var content = await reader.ReadToEndAsync().ConfigureAwait(false);
            bindingContext.Result = ModelBindingResult.Success(content);
        }
    }
}
