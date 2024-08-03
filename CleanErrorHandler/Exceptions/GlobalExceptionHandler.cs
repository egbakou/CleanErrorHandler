using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanErrorHandler.Exceptions;

public class GlobalExceptionHandler(IHostEnvironment hostEnvironment, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionMessage = exception.Message;
        logger.LogError(
            "Error Message: {ExceptionMessage}, Time of occurrence {Time}",
            exceptionMessage, DateTime.UtcNow);
        // Return false to continue with the default behavior
        // - or - return true to signal that this exception is handled
        // return ValueTask.FromResult(false);
        
        var problem = CreateProblemDetails(httpContext, exception);
        var json = ToJson(problem);
        
        const string contentType = "application/problem+json";
        httpContext.Response.ContentType = contentType;
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
    
    private ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred",
            Instance = context.Request.Path.Value,
            Type = Helpers.GetProblemType(StatusCodes.Status500InternalServerError),
        };
        
        problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

        if (hostEnvironment.IsDevelopment())
        {
            problemDetails.Detail = exception.ToString();
            return problemDetails;
        }

        problemDetails.Detail = "An error occurred while processing your request.";
        return problemDetails;
    }

    private string ToJson(in ProblemDetails problemDetails)
    {
        try
        {
            return JsonSerializer.Serialize(problemDetails, SerializerOptions);
        }
        catch (Exception ex)
        {
            const string msg = "An exception has occurred while serializing error to JSON";
            logger.LogError(ex, msg);
        }

        return string.Empty;
    }
    

}