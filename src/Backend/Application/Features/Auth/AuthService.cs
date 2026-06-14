using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Application.Interfaces;
using AlbumFigurinhas.Application.Mappings;
using AlbumFigurinhas.Domain.Entities;
using AlbumFigurinhas.Domain.Exceptions;
using AlbumFigurinhas.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AlbumFigurinhas.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(string login, string senha, CancellationToken ct = default);
    Task<AuthResultDto> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
}

public sealed class AuthService(
    IUsuarioRepository usuarios,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService tokens,
    IUnitOfWork uow,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResultDto> LoginAsync(string login, string senha, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorLoginAsync(login.Trim(), ct);
        if (usuario is null || !hasher.Verificar(senha, usuario.PasswordHash))
            throw new NaoAutorizadoException("Login ou senha inválidos.");

        return await EmitirTokensAsync(usuario, ct);
    }

    public async Task<AuthResultDto> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var hash = tokens.HashRefreshToken(refreshToken);
        var atual = await refreshTokens.ObterPorHashAsync(hash, ct);

        if (atual is null)
            throw new NaoAutorizadoException("Refresh token inválido.");

        if (!atual.EstaAtivo)
        {
            // Reuso de token já rotacionado/revogado: possível roubo — revoga toda a família.
            logger.LogWarning("Reuso de refresh token detectado para o usuário {UsuarioId}; revogando sessões.", atual.UsuarioId);
            await refreshTokens.RevogarTodosDoUsuarioAsync(atual.UsuarioId, ct);
            await uow.SaveChangesAsync(ct);
            throw new NaoAutorizadoException("Refresh token expirado ou revogado.");
        }

        var usuario = await usuarios.ObterPorIdAsync(atual.UsuarioId, ct)
            ?? throw new NaoAutorizadoException("Usuário do token não existe mais.");

        var novoBruto = tokens.GerarRefreshToken();
        var novoHash = tokens.HashRefreshToken(novoBruto);
        atual.Revogar(substituidoPorHash: novoHash);
        await refreshTokens.AdicionarAsync(new RefreshToken(usuario.Id, novoHash, DateTime.UtcNow.AddDays(tokens.RefreshTokenDias)), ct);
        await uow.SaveChangesAsync(ct);

        var (access, expira) = tokens.GerarAccessToken(usuario);
        return new AuthResultDto(access, expira, novoBruto, usuario.ToDto());
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var hash = tokens.HashRefreshToken(refreshToken);
        var atual = await refreshTokens.ObterPorHashAsync(hash, ct);
        if (atual is not null && atual.EstaAtivo)
        {
            atual.Revogar();
            await uow.SaveChangesAsync(ct);
        }
        // logout é idempotente
    }

    private async Task<AuthResultDto> EmitirTokensAsync(Usuario usuario, CancellationToken ct)
    {
        var refreshBruto = tokens.GerarRefreshToken();
        await refreshTokens.AdicionarAsync(
            new RefreshToken(usuario.Id, tokens.HashRefreshToken(refreshBruto), DateTime.UtcNow.AddDays(tokens.RefreshTokenDias)), ct);
        await uow.SaveChangesAsync(ct);

        var (access, expira) = tokens.GerarAccessToken(usuario);
        return new AuthResultDto(access, expira, refreshBruto, usuario.ToDto());
    }
}
