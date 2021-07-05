using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    /// <summary>
    /// Custom model binder to convert an array into a List
    /// </summary>
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(propertyName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "Value is invalid for type List<>");
            }

            return Task.CompletedTask;
        }
    }
}