namespace EF_Core.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity!.IsAuthenticated)
            {
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //await context.Response.WriteAsync("Unauthorized");
                _logger.LogWarning("Unauthenticated request attempt to {Path}", context.Request.Path);
                //return;
            }
            else
            {
                var userName = context.User.Identity.Name;
                _logger.LogInformation("Authenticated request by User with CNIC {User} to {Path}", userName, context.Request.Path);
            }
            await _next(context);
        }
    }

    public static class AuthMiddlewareExtentions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
