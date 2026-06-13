namespace ControleDisciplinas.Api;

/// <summary>
/// Cookie httpOnly que transporta o refresh token (P1 — não é acessível via JS,
/// reduzindo a exposição a XSS). Escopo restrito aos endpoints de autenticação.
/// </summary>
public static class RefreshTokenCookie
{
    public const string Name = "cdu_rt";
    private const string Caminho = "/api/auth";

    public static void Definir(HttpResponse res, string token, int dias, bool isHttps) =>
        res.Cookies.Append(Name, token, Opcoes(isHttps, DateTimeOffset.UtcNow.AddDays(dias)));

    public static void Limpar(HttpResponse res, bool isHttps) =>
        res.Cookies.Append(Name, "", Opcoes(isHttps, DateTimeOffset.UnixEpoch));

    private static CookieOptions Opcoes(bool isHttps, DateTimeOffset expira) => new()
    {
        HttpOnly = true,
        Secure = isHttps,            // exigido em produção (HTTPS); desligado no dev HTTP
        SameSite = SameSiteMode.Lax, // frontend e API são do mesmo site (localhost)
        Path = Caminho,
        Expires = expira,
        IsEssential = true,          // necessário ao funcionamento; isento de consentimento
    };
}
