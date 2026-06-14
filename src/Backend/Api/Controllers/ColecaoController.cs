using ControleDisciplinas.Api.Contracts;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Features.Colecao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDisciplinas.Api.Controllers;

/// <summary>Coleção do colecionador: visualização do álbum e aquisição por tag (PDF §11/§12).</summary>
[ApiController]
[Authorize(Roles = "Colecionador")]
[Route("api/colecao")]
public sealed class ColecaoController(IColecaoService colecao) : ControllerBase
{
    [HttpGet("album")]
    public Task<AlbumColecionadorDto> MeuAlbum(CancellationToken ct) => colecao.MeuAlbumAsync(ct);

    /// <summary>Consulta a figurinha de uma tag (preview antes de adquirir — botão "...").</summary>
    [HttpGet("consultar/{tag}")]
    public async Task<ActionResult<FigurinhaDto>> Consultar(string tag, CancellationToken ct)
    {
        var dto = await colecao.ConsultarPorTagAsync(tag, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("adquirir")]
    public async Task<ActionResult<FigurinhaDto>> Adquirir(AdquirirRequest request, CancellationToken ct)
    {
        var dto = await colecao.AdquirirAsync(request.Tag, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }
}
