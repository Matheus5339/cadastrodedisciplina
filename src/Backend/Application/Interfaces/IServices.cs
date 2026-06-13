using ControleDisciplinas.Domain.Entities;

namespace ControleDisciplinas.Application.Interfaces;

/// <summary>Hash de senha — implementação obrigatória: Argon2id (decisão D8).</summary>
public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verificar(string senha, string hash);
}

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GerarAccessToken(Aluno aluno);
    string GerarRefreshToken();
    string HashRefreshToken(string refreshToken);
    int RefreshTokenDias { get; }
}

/// <summary>Aluno autenticado, extraído exclusivamente do token JWT (regra de segurança 12/13).</summary>
public interface ICurrentUserService
{
    int AlunoId { get; }
}

public sealed record ResultadoImportacaoCsv(bool Executada, string? Arquivo, int Importados, int Ignorados, int LinhasInvalidas, string? Motivo);

public interface ICsvImportService
{
    /// <summary>Importa o catálogo de disciplinas a partir de CSV. Chamado somente na inicialização.</summary>
    Task<ResultadoImportacaoCsv> ImportarAsync(CancellationToken ct = default);
}
