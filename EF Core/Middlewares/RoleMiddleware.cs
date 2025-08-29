namespace EF_Core.Middlewares
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleMiddleware> _logger;

        public RoleMiddleware(RequestDelegate next, ILogger<RoleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(context.User.Identity!.IsAuthenticated)
            {
                var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.ToString();
                _logger.LogInformation("User Role: {Role}", userRole);
            }
            await _next(context);
        }
    }

    public static class RoleMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleMiddleware>();
        }
    }
}
