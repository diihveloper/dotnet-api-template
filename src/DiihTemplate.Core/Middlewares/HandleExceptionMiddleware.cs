using System.Net;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DiihTemplate.Core.Middlewares;

public class HandleExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HandleExceptionMiddleware> _logger;

    public HandleExceptionMiddleware(RequestDelegate next, ILogger<HandleExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotAuthorizedException ex)
        {
            _logger.LogWarning(ex, "Forbidden: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "Not Authorized", HttpStatusCode.Forbidden);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Entity not found: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "Entity Not Found", HttpStatusCode.NotFound);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "File not found: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "File Not Found", HttpStatusCode.NotFound);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (EntityAlreadyExistsException ex)
        {
            _logger.LogWarning(ex, "Entity already exists: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "Entity Already Exists", HttpStatusCode.Conflict);
        }
        catch (NotImplementedException ex)
        {
            _logger.LogWarning(ex, "Not implemented: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "Not Implemented", HttpStatusCode.NotImplemented);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "Unauthorized", HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal Server Error: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, "Internal Server Error");
        }
    }


    private async Task HandleExceptionAsync(HttpContext context, string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        context.Response.Headers.Remove("Cache-Control");
        await context.Response.WriteAsync(ResultDto.Error(message, statusCode));
    }

}