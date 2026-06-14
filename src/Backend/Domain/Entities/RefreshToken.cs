using AlbumFigurinhas.Shared.Kernel;

namespace AlbumFigurinhas.Domain.Entities;

/// <summary>
/// Tabela técnica: refresh token persistido apenas como hash, com suporte a
/// rotação (ReplacedByTokenHash) e revogação (RevokedAtUtc).
/// </summary>
public class RefreshToken : EntityBase
{
    public int UsuarioId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public Usuario? Usuario { get; private set; }

    public bool EstaAtivo => RevokedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;

    private RefreshToken() { } // EF Core

    public RefreshToken(int usuarioId, string tokenHash, DateTime expiresAtUtc)
    {
        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Revogar(string? substituidoPorHash = null)
    {
        RevokedAtUtc ??= DateTime.UtcNow;
        ReplacedByTokenHash ??= substituidoPorHash;
    }
}
