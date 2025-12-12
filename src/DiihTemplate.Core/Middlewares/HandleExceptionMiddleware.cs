using System.Net;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DiihTemplate.Core.Middlewares;

public class HandleExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public HandleExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotAuthorizedException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.Headers.Remove("Cache-Control");
            await context.Response.WriteAsync(
                ResultDto.Error("Not Authorized", HttpStatusCode.Forbidden));
        }
        catch (EntityNotFoundException ex)
        {
            await HandleExceptionAsync(context, "Entity Not Found", HttpStatusCode.NotFound);
        }
        catch (BusinessException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (FileNotFoundException ex)
        {
            await HandleExceptionAsync(context, "File Not Found", HttpStatusCode.NotFound);
        }
        catch (BadRequestException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (EntityAlreadyExistsException ex)
        {
            await HandleExceptionAsync(context, "Entity Already Exists", HttpStatusCode.Conflict);
        }
        catch (NotImplementedException ex)
        {
            await HandleExceptionAsync(context, "Not Implemented", HttpStatusCode.NotImplemented);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, "Unauthorized", HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            //Log.Logger.Error(ex, "Internal Server Error");
            await HandleExceptionAsync(context, new Exception("Internal Server Error", ex));
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.Headers.Remove("Cache-Control");
        await context.Response.WriteAsync(ResultDto.Error(exception.Message));
    }
}