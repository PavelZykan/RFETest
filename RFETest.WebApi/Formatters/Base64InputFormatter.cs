using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using RFETest.Utils;
using RFETest.WebContracts;
using System.Text.Json;

namespace RFETest.WebApi.Formatters
{
    /// <summary>
    /// Will convert base64 input to a json object
    /// </summary>
    public class Base64InputFormatter : InputFormatter
    {
        public Base64InputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/custom"));
        }

        public override Boolean CanRead(InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;

            if (contentType == "application/custom")
            {
                return true;
            }

            return false;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;

            if (contentType == "application/custom")
            {
                using var streamReader = new StreamReader(request.Body);

                var originalContent = await streamReader.ReadToEndAsync();

                var data = Convert.FromBase64String(originalContent);
                var decodedString = System.Text.Encoding.UTF8.GetString(data);

                // TODO: the type is hardcoded here since it is the only accepted input
                var inputObject = JsonSerializer.Deserialize<DiffInput>(decodedString, SerializationUtils.Options);

                return await InputFormatterResult.SuccessAsync(inputObject);
            }

            return await InputFormatterResult.FailureAsync();
        }
    }
}
