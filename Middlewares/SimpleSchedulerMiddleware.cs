namespace SimpleScheduler.Middlewares;

public class SimpleSchedulerMiddleware : IMiddleware
{
    private readonly SimpleSchedulerUser _user;
    
    public SimpleSchedulerMiddleware(SimpleSchedulerUser user)
    {
        _user = user;
    }
    
    /// <inheritdoc /> 
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isLoginPath = context.Request.Path.Value?.Contains("login") ?? false;
        if (!isLoginPath &&
            (!context.Request.Cookies.TryGetValue(Constants.USER_COOKIE, out var value)
            || value != _user.Username))
        {
            context.Response.Redirect("/simple-scheduler/login");
            return;
        }
        
        await next(context);
    }
}