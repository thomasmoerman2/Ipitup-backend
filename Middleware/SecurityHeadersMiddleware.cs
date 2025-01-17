namespace Ipitup_backend.Middleware;
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        // HSTS
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        // Prevent MIME type sniffing
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        // Referrer Policy
        context.Response.Headers.Add("Referrer-Policy", "no-referrer");
        // XSS Protection
        context.Response.Headers.Add("X-XSS-Protection", "0");
        // X-Frame-Options (Clickjacking protection)
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        // Content Security Policy
        context.Response.Headers.Add(
            "Content-Security-Policy",
            "default-src 'self'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "frame-ancestors 'none';"
        );
        await _next(context);
    }
}
