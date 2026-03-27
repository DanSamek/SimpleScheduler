using SimpleScheduler.Services;

namespace SimpleScheduler.Middlewares;

public class SimpleSchedulerMiddleware : IMiddleware
{
    private readonly ITokenService _tokenService;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerMiddleware(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    /// <inheritdoc /> 
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isLoginPath = context.Request.Path.Value?.Contains("login") ?? false;
        if (!isLoginPath &&
            (!context.Request.Cookies.TryGetValue(Constants.USER_COOKIE, out var value)
            || !await _tokenService.ValidateToken(value)))
        {
            context.Response.Redirect("/simple-scheduler/login");
            return;
        }
        
        await next(context);
    }
}