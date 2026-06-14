using AlbumFigurinhas.Api.Contracts;
using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Application.Features.Album;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlbumFigurinhas.Api.Controllers;

/// <summary>Álbum único da aplicação (PDF §8 — FrmAutoria / capa).</summary>
[ApiController]
[Authorize]
[Route("api/album")]
public sealed class AlbumController(IAlbumService album) : ControllerBase
{
    [HttpGet]
    public Task<AlbumDto> Obter(CancellationToken ct) => album.ObterAsync(ct);

    [HttpPut]
    [Authorize(Roles = "Autor")]
    public Task<AlbumDto> Atualizar(AtualizarAlbumRequest request, CancellationToken ct) =>
        album.AtualizarAsync(request.Nome, request.Paginas, ct);

    [HttpGet("capa")]
    public async Task<IActionResult> ObterCapa(CancellationToken ct)
    {
        var img = await album.ObterCapaAsync(ct);
        return File(img.Conteudo, img.ContentType);
    }

    [HttpPut("capa")]
    [Authorize(Roles = "Autor")]
    [RequestSizeLimit(4 * 1024 * 1024)]
    public async Task<IActionResult> EnviarCapa(IFormFile imagem, CancellationToken ct)
    {
        if (imagem is null || imagem.Length == 0)
            throw new Domain.Exceptions.ValidacaoException("Nenhuma imagem enviada (campo 'imagem').");
        using var ms = new MemoryStream();
        await imagem.CopyToAsync(ms, ct);
        await album.DefinirCapaAsync(ms.ToArray(), imagem.ContentType, ct);
        return NoContent();
    }
}
