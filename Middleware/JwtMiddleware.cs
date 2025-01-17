namespace Ipitup_backend.Middleware;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;
    public JwtMiddleware(RequestDelegate next, IJwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null && _jwtService.ValidateToken(token))
        {
            var userId = _jwtService.GetUserIdFromToken(token);
            if (userId.HasValue)
            {
                context.Items["UserId"] = userId.Value;
            }
        }
        await _next(context);
    }
}
