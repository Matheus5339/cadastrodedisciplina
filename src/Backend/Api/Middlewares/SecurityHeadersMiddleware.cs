namespace AlbumFigurinhas.Api.Middlewares;

/// <summary>
/// Cabeçalhos de segurança HTTP (P0 — hardening). Como a API serve apenas JSON,
/// a política padrão é restritiva: nega recursos ativos e enquadramento em iframes.
/// A Swagger UI (somente em desenvolvimento) recebe um CSP relaxado, pois carrega
/// seus próprios scripts/estilos.
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

        h["Content-Security-Policy"] = context.Request.Path.StartsWithSegments("/swagger")
            ? "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:"
            : "default-src 'none'; frame-ancestors 'none'";

        return next(context);
    }
}
