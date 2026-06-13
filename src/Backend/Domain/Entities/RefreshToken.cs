using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

/// <summary>
/// Tabela técnica (decisão D7): refresh token persistido apenas como hash,
/// com suporte a rotação (ReplacedByTokenHash) e revogação (RevokedAtUtc).
/// </summary>
public class RefreshToken : EntityBase
{
    public int AlunoId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public Aluno? Aluno { get; private set; }

    public bool EstaAtivo => RevokedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;

    private RefreshToken() { } // EF Core

    public RefreshToken(int alunoId, string tokenHash, DateTime expiresAtUtc)
    {
        AlunoId = alunoId;
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
