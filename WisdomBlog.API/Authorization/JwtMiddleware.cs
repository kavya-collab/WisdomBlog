using WisdomBlog.API.Services;

namespace WisdomBlog.API.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var response = jwtUtils.ValidateToken(token);
            if (response.Id.HasValue)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = userService.GetById(response.Id.Value);
            }
            await _next(context);
        }
    }
}
