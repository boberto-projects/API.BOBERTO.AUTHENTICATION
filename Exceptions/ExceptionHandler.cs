﻿using Microsoft.AspNetCore.Diagnostics;

namespace api_authentication_boberto.Exceptions
{
    public static class ExceptionHandler
    {
        public static void AddCustomExceptionHandler(this IApplicationBuilder exception)
        {
            exception.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is CustomException customException)
                {
                    context.Response.StatusCode = customException.CodigoDeStatus;
                    await context.Response.WriteAsJsonAsync(customException.ObterResponse());
                }
                if (exceptionHandlerPathFeature?.Error is CodigoOTPException codigoOTPException)
                {
                    context.Response.StatusCode = codigoOTPException.CodigoDeStatus;
                    await context.Response.WriteAsJsonAsync(codigoOTPException.ObterResponse());
                }
            });
        }
    }
}