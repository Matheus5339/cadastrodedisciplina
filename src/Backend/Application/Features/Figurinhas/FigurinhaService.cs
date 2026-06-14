using System.Security.Cryptography;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;
using ControleDisciplinas.Shared.Constants;

namespace ControleDisciplinas.Application.Features.Figurinhas;

public interface IFigurinhaService
{
    Task<IReadOnlyList<FigurinhaDto>> ListarAsync(string? texto, int? pagina, CancellationToken ct = default);
    Task<FigurinhaDto> ObterAsync(int id, CancellationToken ct = default);
    Task<FigurinhaDto?> ObterPorTagAsync(string tag, CancellationToken ct = default);
    Task<FigurinhaDto> CriarAsync(int numero, string nome, int pagina, string? descricao, byte[] imagem, string contentType, CancellationToken ct = default);
    Task<FigurinhaDto> AtualizarAsync(int id, int numero, string nome, int pagina, string? descricao, byte[]? imagem, string? contentType, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
    Task LimparTodasAsync(CancellationToken ct = default);
    Task<ImagemDto> ObterImagemAsync(int id, CancellationToken ct = default);
}

public sealed class FigurinhaService(
    IFigurinhaRepository figurinhas,
    IAlbumRepository albuns,
    IUnitOfWork uow) : IFigurinhaService
{
    public async Task<IReadOnlyList<FigurinhaDto>> ListarAsync(string? texto, int? pagina, CancellationToken ct = default)
    {
        var album = await ObterAlbumAsync(ct);
        var lista = await figurinhas.ListarAsync(album.Id, new FigurinhaFiltro(texto, pagina), ct);
        return lista.Select(f => f.ToDto()).ToList();
    }

    public async Task<FigurinhaDto> ObterAsync(int id, CancellationToken ct = default) =>
        (await ObterEntidadeAsync(id, ct)).ToDto();

    public async Task<FigurinhaDto?> ObterPorTagAsync(string tag, CancellationToken ct = default)
    {
        var f = await figurinhas.ObterPorTagAsync((tag ?? string.Empty).Trim().ToLowerInvariant(), ct);
        return f?.ToDto();
    }

    public async Task<FigurinhaDto> CriarAsync(int numero, string nome, int pagina, string? descricao, byte[] imagem, string contentType, CancellationToken ct = default)
    {
        var album = await ObterAlbumAsync(ct);
        ValidarPaginaEImagem(pagina, album.Paginas, contentType);

        if (await figurinhas.ExisteNumeroAsync(album.Id, numero, ct: ct))
            throw new ConflitoException($"Já existe uma figurinha com o número {numero} no álbum.");

        var tag = CalcularTag(imagem);
        if (await figurinhas.ExisteTagAsync(tag, ct: ct))
            throw new ConflitoException("Já existe uma figurinha com esta mesma imagem (tag duplicada).");

        var figurinha = new Figurinha(album.Id, numero, nome, pagina, descricao);
        figurinha.DefinirImagem(imagem, contentType, tag, FotoConstants.TamanhoMaximoBytes);

        await figurinhas.AdicionarAsync(figurinha, ct);
        await uow.SaveChangesAsync(ct);
        return figurinha.ToDto();
    }

    public async Task<FigurinhaDto> AtualizarAsync(int id, int numero, string nome, int pagina, string? descricao, byte[]? imagem, string? contentType, CancellationToken ct = default)
    {
        var album = await ObterAlbumAsync(ct);
        var figurinha = await ObterEntidadeAsync(id, ct);

        if (await figurinhas.ExisteNumeroAsync(album.Id, numero, id, ct))
            throw new ConflitoException($"Já existe uma figurinha com o número {numero} no álbum.");

        if (pagina > album.Paginas)
            throw new ValidacaoException($"A página deve estar entre 1 e {album.Paginas}.");

        figurinha.Atualizar(numero, nome, pagina, descricao);

        if (imagem is { Length: > 0 })
        {
            ValidarPaginaEImagem(pagina, album.Paginas, contentType);
            var tag = CalcularTag(imagem);
            if (await figurinhas.ExisteTagAsync(tag, id, ct))
                throw new ConflitoException("Já existe uma figurinha com esta mesma imagem (tag duplicada).");
            figurinha.DefinirImagem(imagem, contentType!, tag, FotoConstants.TamanhoMaximoBytes);
        }

        await uow.SaveChangesAsync(ct);
        return figurinha.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var figurinha = await ObterEntidadeAsync(id, ct);
        figurinhas.Remover(figurinha);
        await uow.SaveChangesAsync(ct);
    }

    public async Task LimparTodasAsync(CancellationToken ct = default)
    {
        var album = await ObterAlbumAsync(ct);
        await figurinhas.RemoverTodasAsync(album.Id, ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<ImagemDto> ObterImagemAsync(int id, CancellationToken ct = default)
    {
        var figurinha = await ObterEntidadeAsync(id, ct);
        if (figurinha.Imagem is not { Length: > 0 } || figurinha.ImagemContentType is null)
            throw new NaoEncontradoException("A figurinha não possui imagem.");
        return new ImagemDto(figurinha.Imagem, figurinha.ImagemContentType);
    }

    private static string CalcularTag(byte[] imagem) =>
        Convert.ToHexString(MD5.HashData(imagem)).ToLowerInvariant();

    private static void ValidarPaginaEImagem(int pagina, int totalPaginas, string? contentType)
    {
        if (pagina > totalPaginas)
            throw new ValidacaoException($"A página deve estar entre 1 e {totalPaginas}.");
        if (!FotoConstants.TiposPermitidos.ContainsKey(contentType ?? string.Empty))
            throw new ValidacaoException("Tipo de imagem não permitido (use JPEG, PNG ou WebP).");
    }

    private async Task<Domain.Entities.Album> ObterAlbumAsync(CancellationToken ct) =>
        await albuns.ObterAsync(ct) ?? throw new NaoEncontradoException("Álbum não encontrado.");

    private async Task<Figurinha> ObterEntidadeAsync(int id, CancellationToken ct) =>
        await figurinhas.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Figurinha não encontrada.");
}
