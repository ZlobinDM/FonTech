using System.Net;
using FonTech.Domain.Enum;
using FonTech.Domain.Result;
using Serilog;
using ILogger = Serilog.ILogger;


namespace FonTech.Api.MiddleWares;

public class ExcetionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExcetionHandlingMiddleware(ILogger logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        _logger.Error(ex, ex.Message);
        var errorMessage = ex.Message;

        var responce = ex switch
        {
            UnauthorizedAccessException _ => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int)HttpStatusCode.Unauthorized },
            _ => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int)HttpStatusCode.InternalServerError },
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)responce.ErrorCode;
        await httpContext.Response.WriteAsJsonAsync(responce);
    }
}