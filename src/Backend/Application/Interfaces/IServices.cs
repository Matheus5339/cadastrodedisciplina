using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Enums;

namespace ControleDisciplinas.Application.Interfaces;

/// <summary>Hash de senha — implementação obrigatória: Argon2id.</summary>
public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verificar(string senha, string hash);
}

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GerarAccessToken(Usuario usuario);
    string GerarRefreshToken();
    string HashRefreshToken(string refreshToken);
    int RefreshTokenDias { get; }
}

/// <summary>Usuário autenticado, extraído exclusivamente do token JWT.</summary>
public interface ICurrentUserService
{
    int UsuarioId { get; }
    Perfil Perfil { get; }
}
