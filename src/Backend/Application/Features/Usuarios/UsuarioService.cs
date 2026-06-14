using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Application.Validators;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Enums;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;

namespace ControleDisciplinas.Application.Features.Usuarios;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ListarAsync(string? filtro, CancellationToken ct = default);
    Task<UsuarioDto> ObterAsync(int id, CancellationToken ct = default);
    Task<UsuarioDto> CriarAsync(string nome, string senha, Perfil perfil, CancellationToken ct = default);
    Task<UsuarioDto> AtualizarAsync(int id, string nome, Perfil perfil, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
    /// <summary>Admin zera a senha do usuário para um padrão e devolve a senha gerada (assignment §adm).</summary>
    Task<string> ResetarSenhaAsync(int id, CancellationToken ct = default);
    Task<UsuarioDto> AtualizarProprioAsync(string nome, CancellationToken ct = default);
    Task TrocarSenhaAsync(string novaSenha, CancellationToken ct = default);
}

public sealed class UsuarioService(
    IUsuarioRepository usuarios,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    ICurrentUserService atual,
    IUnitOfWork uow) : IUsuarioService
{
    /// <summary>Senha padrão gerada ao zerar a senha de um usuário (admin).</summary>
    public const string SenhaPadrao = "123456";

    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(string? filtro, CancellationToken ct = default) =>
        (await usuarios.ListarAsync(filtro, ct)).Select(u => u.ToDto()).ToList();

    public async Task<UsuarioDto> ObterAsync(int id, CancellationToken ct = default)
    {
        var u = await usuarios.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Usuário não encontrado.");
        return u.ToDto();
    }

    public async Task<UsuarioDto> CriarAsync(string nome, string senha, Perfil perfil, CancellationToken ct = default)
    {
        SenhaValidator.Validar(senha);
        var usuario = new Usuario(nome, hasher.Hash(senha), perfil);

        if (await usuarios.ExisteNomeAsync(usuario.Nome, ct: ct))
            throw new ConflitoException("Já existe um usuário com este nome.");

        await usuarios.AdicionarAsync(usuario, ct);
        await uow.SaveChangesAsync(ct);
        return usuario.ToDto();
    }

    public async Task<UsuarioDto> AtualizarAsync(int id, string nome, Perfil perfil, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Usuário não encontrado.");
        if (await usuarios.ExisteNomeAsync(nome.Trim(), id, ct))
            throw new ConflitoException("Já existe um usuário com este nome.");

        usuario.Atualizar(nome, perfil);
        await uow.SaveChangesAsync(ct);
        return usuario.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        if (id == atual.UsuarioId)
            throw new ConflitoException("Você não pode remover o próprio usuário.");

        var usuario = await usuarios.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Usuário não encontrado.");
        await refreshTokens.RevogarTodosDoUsuarioAsync(id, ct);
        usuarios.Remover(usuario);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<string> ResetarSenhaAsync(int id, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Usuário não encontrado.");
        usuario.DefinirPasswordHash(hasher.Hash(SenhaPadrao));
        await refreshTokens.RevogarTodosDoUsuarioAsync(id, ct);
        await uow.SaveChangesAsync(ct);
        return SenhaPadrao;
    }

    public async Task<UsuarioDto> AtualizarProprioAsync(string nome, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(atual.UsuarioId, ct) ?? throw new NaoEncontradoException("Usuário não encontrado.");
        if (await usuarios.ExisteNomeAsync(nome.Trim(), usuario.Id, ct))
            throw new ConflitoException("Já existe um usuário com este nome.");

        usuario.AtualizarProprioNome(nome); // o perfil não muda (PDF §7)
        await uow.SaveChangesAsync(ct);
        return usuario.ToDto();
    }

    public async Task TrocarSenhaAsync(string novaSenha, CancellationToken ct = default)
    {
        SenhaValidator.Validar(novaSenha);
        var usuario = await usuarios.ObterPorIdAsync(atual.UsuarioId, ct) ?? throw new NaoEncontradoException("Usuário não encontrado.");
        usuario.DefinirPasswordHash(hasher.Hash(novaSenha));
        await refreshTokens.RevogarTodosDoUsuarioAsync(usuario.Id, ct);
        await uow.SaveChangesAsync(ct);
    }
}
