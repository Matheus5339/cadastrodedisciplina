namespace AlbumFigurinhas.Api;

/// <summary>Nomes das políticas de rate limiting (P0).</summary>
public static class RateLimitPolicies
{
    /// <summary>Limite para endpoints sensíveis de autenticação (anti brute-force).</summary>
    public const string Auth = "auth";
}
