namespace ControleDisciplinas.Infrastructure.Auth;

/// <summary>
/// Configuração do JWT. O Secret NUNCA fica em arquivo versionado:
/// vem de variável de ambiente (Jwt__Secret), user secrets ou appsettings.Development.json.
/// </summary>
public sealed class JwtOptions
{
    public const string Secao = "Jwt";
    public const int TamanhoMinimoSecret = 32;

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "controle-disciplinas-ucp";
    public string Audience { get; set; } = "controle-disciplinas-ucp-front";
    public int AccessTokenMinutos { get; set; } = 15;
    public int RefreshTokenDias { get; set; } = 7;
}
