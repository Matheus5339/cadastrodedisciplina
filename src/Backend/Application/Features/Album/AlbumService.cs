using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;
using ControleDisciplinas.Shared.Constants;

namespace ControleDisciplinas.Application.Features.Album;

public interface IAlbumService
{
    Task<AlbumDto> ObterAsync(CancellationToken ct = default);
    Task<AlbumDto> AtualizarAsync(string nome, int paginas, CancellationToken ct = default);
    Task DefinirCapaAsync(byte[] conteudo, string contentType, CancellationToken ct = default);
    Task<ImagemDto> ObterCapaAsync(CancellationToken ct = default);
}

public sealed class AlbumService(IAlbumRepository albuns, IUnitOfWork uow) : IAlbumService
{
    public async Task<AlbumDto> ObterAsync(CancellationToken ct = default) =>
        (await ObterEntidadeAsync(ct)).ToDto();

    public async Task<AlbumDto> AtualizarAsync(string nome, int paginas, CancellationToken ct = default)
    {
        var album = await ObterEntidadeAsync(ct);
        album.Atualizar(nome, paginas);
        await uow.SaveChangesAsync(ct);
        return album.ToDto();
    }

    public async Task DefinirCapaAsync(byte[] conteudo, string contentType, CancellationToken ct = default)
    {
        if (!FotoConstants.TiposPermitidos.ContainsKey(contentType ?? string.Empty))
            throw new ValidacaoException("Tipo de imagem não permitido (use JPEG, PNG ou WebP).");

        var album = await ObterEntidadeAsync(ct);
        album.DefinirCapa(conteudo, contentType!, FotoConstants.TamanhoMaximoBytes);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<ImagemDto> ObterCapaAsync(CancellationToken ct = default)
    {
        var album = await ObterEntidadeAsync(ct);
        if (album.Capa is not { Length: > 0 } || album.CapaContentType is null)
            throw new NaoEncontradoException("O álbum não possui capa.");
        return new ImagemDto(album.Capa, album.CapaContentType);
    }

    private async Task<Domain.Entities.Album> ObterEntidadeAsync(CancellationToken ct) =>
        await albuns.ObterAsync(ct) ?? throw new NaoEncontradoException("Álbum não encontrado.");
}
