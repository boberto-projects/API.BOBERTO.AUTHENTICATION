using Microsoft.AspNetCore.Diagnostics;

namespace api_authentication_boberto.Exceptions
{
    public static class ExceptionHandler
    {
        public static void AddCustomExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(builder =>
            builder.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                ///TODO: DELEGATE THIS TO A CUSTOM ERROR MIDDLEWARE AND UNDONE THIS multiple IFS. 
                /// THIS IS THE GO HORSE PROBLEM!
                if (exceptionHandlerPathFeature?.Error is CustomException customException)
                {
                    context.Response.StatusCode = customException.CodigoDeStatus;
                    await context.Response.WriteAsJsonAsync(customException.ObterResponse());
                }
                if (exceptionHandlerPathFeature?.Error is ApiKeyAuthenticationException apiKeyAuthenticationException)
                {
                    var apiKeyException = apiKeyAuthenticationException.GetResponse();
                    context.Response.StatusCode = apiKeyException.StatusCode;
                    await context.Response.WriteAsJsonAsync(apiKeyException);

                }
                if (exceptionHandlerPathFeature?.Error is CodeOTPException codigoOTPException)
                {
                    context.Response.StatusCode = codigoOTPException.CodigoDeStatus;
                    await context.Response.WriteAsJsonAsync(codigoOTPException.ObterResponse());
                }
            }));
        }

    }
}
