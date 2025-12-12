using DiihTemplate.Core.Repositories;
using Microsoft.AspNetCore.Http;

namespace DiihTemplate.Core.Middlewares;

public class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
    {
        if (context.Request.Method is "GET" or "HEAD" or "OPTIONS")
        {
            await _next(context);
            return;
        }

        try
        {
            await unitOfWork.BeginTransactionAsync();
            await _next(context); // passa para o controller
            await unitOfWork.CommitAsync();
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}