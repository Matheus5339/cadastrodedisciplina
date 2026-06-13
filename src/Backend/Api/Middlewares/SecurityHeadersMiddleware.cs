namespace ControleDisciplinas.Api.Middlewares;

/// <summary>
/// Cabeçalhos de segurança HTTP (P0 — hardening). Como a API serve apenas JSON,
/// a política é restritiva: nega recursos ativos e enquadramento em iframes.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        var h = context.Response.Headers;
        h["X-Content-Type-Options"] = "nosniff";
        h["X-Frame-Options"] = "DENY";
        h["Referrer-Policy"] = "no-referrer";
        h["X-Permitted-Cross-Domain-Policies"] = "none";
        h["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
        h["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
        return next(context);
    }
}
