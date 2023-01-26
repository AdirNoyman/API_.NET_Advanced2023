using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseLibrary.API.Helpers
{
    // The model binder, maps the http request data to a POCO
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // The binder will work only on enumarble data types, so we need to make sure that we are apssing to it enumarble type
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // Get the inputted value through the value provider
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            // If that value is null or whitespace, we return
            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // if The value isn't null or whitespace and the type of the model is enumarble
            // Get the enumabrble's type, and data type converter for it
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            // Create a type converter
            var converter = TypeDescriptor.GetConverter(elementType);

            // Convert each item in the value list to the enumarble type
            // Taking each Guid we got in the request url and convert it to a GUID from the string
            var values = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => converter.ConvertFromString(x.Trim())).ToArray();

            // Create an array of that type, and set it as a model value
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            bindingContext.Model = typedValues;

            // return a successful result, passing in the model
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}