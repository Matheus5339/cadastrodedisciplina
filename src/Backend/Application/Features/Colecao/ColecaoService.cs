using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;

namespace ControleDisciplinas.Application.Features.Colecao;

public interface IColecaoService
{
    /// <summary>Adquire uma figurinha pela tag (PDF §12 — FrmNovaFigurinha).</summary>
    Task<FigurinhaDto> AdquirirAsync(string tag, CancellationToken ct = default);

    /// <summary>Álbum do colecionador: figurinhas do catálogo marcadas como adquiridas ou não.</summary>
    Task<AlbumColecionadorDto> MeuAlbumAsync(CancellationToken ct = default);

    /// <summary>Consulta uma figurinha pela tag (preview antes de adquirir).</summary>
    Task<FigurinhaDto?> ConsultarPorTagAsync(string tag, CancellationToken ct = default);
}

public sealed class ColecaoService(
    IFigurinhaRepository figurinhas,
    IFigurinhaAdquiridaRepository adquiridas,
    IAlbumRepository albuns,
    ICurrentUserService atual,
    IUnitOfWork uow) : IColecaoService
{
    public async Task<FigurinhaDto?> ConsultarPorTagAsync(string tag, CancellationToken ct = default)
    {
        var f = await figurinhas.ObterPorTagAsync(Normalizar(tag), ct);
        return f?.ToDto();
    }

    public async Task<FigurinhaDto> AdquirirAsync(string tag, CancellationToken ct = default)
    {
        var figurinha = await figurinhas.ObterPorTagAsync(Normalizar(tag), ct)
            ?? throw new NaoEncontradoException("Nenhuma figurinha encontrada com esta tag.");

        if (await adquiridas.ExisteAsync(atual.UsuarioId, figurinha.Id, ct))
            throw new ConflitoException("Você já possui esta figurinha.");

        await adquiridas.AdicionarAsync(new FigurinhaAdquirida(atual.UsuarioId, figurinha.Id), ct);
        await uow.SaveChangesAsync(ct);
        return figurinha.ToDto();
    }

    public async Task<AlbumColecionadorDto> MeuAlbumAsync(CancellationToken ct = default)
    {
        var album = await albuns.ObterAsync(ct) ?? throw new NaoEncontradoException("Álbum não encontrado.");
        var catalogo = await figurinhas.ListarAsync(album.Id, new FigurinhaFiltro(null, null), ct);
        var minhas = (await adquiridas.ListarDoUsuarioAsync(atual.UsuarioId, ct))
            .Select(a => a.FigurinhaId)
            .ToHashSet();

        var figs = catalogo
            .Select(f => f.ToAlbumDto(minhas.Contains(f.Id)))
            .ToList();

        return new AlbumColecionadorDto(album.ToDto(), figs);
    }

    private static string Normalizar(string? tag) => (tag ?? string.Empty).Trim().ToLowerInvariant();
}
