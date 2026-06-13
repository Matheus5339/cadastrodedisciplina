using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Application.Validators;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ControleDisciplinas.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResultDto> RegistrarAsync(string rgu, string cpf, string email, string nome, string senha, CancellationToken ct = default);
    Task<AuthResultDto> LoginAsync(string email, string senha, CancellationToken ct = default);
    Task<AuthResultDto> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
}

public sealed class AuthService(
    IAlunoRepository alunos,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService tokens,
    IUnitOfWork uow,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResultDto> RegistrarAsync(string rgu, string cpf, string email, string nome, string senha, CancellationToken ct = default)
    {
        SenhaValidator.Validar(senha);

        // entidade valida RGU, CPF, e-mail e nome via value objects
        var aluno = new Aluno(rgu, cpf, email, nome, hasher.Hash(senha));

        if (await alunos.ExisteEmailAsync(aluno.Email, ct: ct))
            throw new ConflitoException("Já existe um aluno cadastrado com este e-mail.");
        if (await alunos.ExisteCpfAsync(aluno.Cpf, ct: ct))
            throw new ConflitoException("Já existe um aluno cadastrado com este CPF.");
        if (await alunos.ExisteRguAsync(aluno.Rgu, ct: ct))
            throw new ConflitoException("Já existe um aluno cadastrado com este RGU.");

        await alunos.AdicionarAsync(aluno, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Novo aluno cadastrado: {AlunoId}", aluno.Id);
        return await EmitirTokensAsync(aluno, ct);
    }

    public async Task<AuthResultDto> LoginAsync(string email, string senha, CancellationToken ct = default)
    {
        var aluno = await alunos.ObterPorEmailAsync(email.Trim().ToLowerInvariant(), ct);
        if (aluno is null || !hasher.Verificar(senha, aluno.PasswordHash))
            throw new NaoAutorizadoException("E-mail ou senha inválidos.");

        return await EmitirTokensAsync(aluno, ct);
    }

    public async Task<AuthResultDto> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var hash = tokens.HashRefreshToken(refreshToken);
        var atual = await refreshTokens.ObterPorHashAsync(hash, ct);

        if (atual is null)
            throw new NaoAutorizadoException("Refresh token inválido.");

        if (!atual.EstaAtivo)
        {
            // Reuso de token já rotacionado/revogado: possível roubo — revoga toda a família (segurança 3)
            logger.LogWarning("Reuso de refresh token detectado para o aluno {AlunoId}; revogando sessões.", atual.AlunoId);
            await refreshTokens.RevogarTodosDoAlunoAsync(atual.AlunoId, ct);
            await uow.SaveChangesAsync(ct);
            throw new NaoAutorizadoException("Refresh token expirado ou revogado.");
        }

        var aluno = await alunos.ObterPorIdAsync(atual.AlunoId, ct)
            ?? throw new NaoAutorizadoException("Aluno do token não existe mais.");

        var novoBruto = tokens.GerarRefreshToken();
        var novoHash = tokens.HashRefreshToken(novoBruto);
        atual.Revogar(substituidoPorHash: novoHash);
        await refreshTokens.AdicionarAsync(new RefreshToken(aluno.Id, novoHash, DateTime.UtcNow.AddDays(tokens.RefreshTokenDias)), ct);
        await uow.SaveChangesAsync(ct);

        var (access, expira) = tokens.GerarAccessToken(aluno);
        return new AuthResultDto(access, expira, novoBruto, aluno.ToDto());
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
        // logout é idempotente: token desconhecido não revela nada
    }

    private async Task<AuthResultDto> EmitirTokensAsync(Aluno aluno, CancellationToken ct)
    {
        var refreshBruto = tokens.GerarRefreshToken();
        await refreshTokens.AdicionarAsync(
            new RefreshToken(aluno.Id, tokens.HashRefreshToken(refreshBruto), DateTime.UtcNow.AddDays(tokens.RefreshTokenDias)), ct);
        await uow.SaveChangesAsync(ct);

        var (access, expira) = tokens.GerarAccessToken(aluno);
        return new AuthResultDto(access, expira, refreshBruto, aluno.ToDto());
    }
}
