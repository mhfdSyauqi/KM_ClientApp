
using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Policy;
using System.Net;
using System.Text.Json;

namespace KM_ClientApp.Middleware;

public class ErrorExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            await HandleValidationExecptionAsync(context, e);
        }
        catch (Exception e)
        {
            await HandleExecptionAsync(context, e);
        }
    }

    private static Task HandleExecptionAsync(HttpContext context, Exception exception)
    {
        var jsonOpt = new JsonSerializerOptions() { PropertyNamingPolicy = new JsonLowerCaseKeyPolicy() };
        var errResponse = new HttpResponseError(exception.Message);
        var result = JsonSerializer.Serialize(errResponse, jsonOpt);
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(result);
    }

    private static Task HandleValidationExecptionAsync(HttpContext context, ValidationException exception)
    {
        var jsonOpt = new JsonSerializerOptions() { PropertyNamingPolicy = new JsonLowerCaseKeyPolicy() };
        var errMsg = exception.Errors.First().ErrorMessage;
        var errResponse = new HttpResponseError(errMsg);
        var result = JsonSerializer.Serialize(errResponse, jsonOpt);
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(result);
    }
}
