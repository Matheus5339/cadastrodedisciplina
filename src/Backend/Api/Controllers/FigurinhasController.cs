using AlbumFigurinhas.Api.Contracts;
using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Application.Features.Figurinhas;
using AlbumFigurinhas.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlbumFigurinhas.Api.Controllers;

/// <summary>Figurinhas do álbum (PDF §8/§9). Leitura por qualquer perfil; escrita só do Autor.</summary>
[ApiController]
[Authorize]
[Route("api/figurinhas")]
public sealed class FigurinhasController(IFigurinhaService figurinhas) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FigurinhaDto>>> Listar(
        [FromQuery] string? texto, [FromQuery] int? pagina, CancellationToken ct) =>
        Ok(await figurinhas.ListarAsync(texto, pagina, ct));

    [HttpGet("{id:int}")]
    public Task<FigurinhaDto> Obter(int id, CancellationToken ct) => figurinhas.ObterAsync(id, ct);

    [HttpGet("{id:int}/imagem")]
    public async Task<IActionResult> ObterImagem(int id, CancellationToken ct)
    {
        var img = await figurinhas.ObterImagemAsync(id, ct);
        return File(img.Conteudo, img.ContentType);
    }

    [HttpPost]
    [Authorize(Roles = "Autor")]
    [RequestSizeLimit(4 * 1024 * 1024)]
    public async Task<ActionResult<FigurinhaDto>> Criar([FromForm] FigurinhaFormRequest request, CancellationToken ct)
    {
        if (request.Imagem is null || request.Imagem.Length == 0)
            throw new ValidacaoException("A imagem da figurinha é obrigatória (campo 'imagem').");
        var bytes = await LerAsync(request.Imagem, ct);
        var dto = await figurinhas.CriarAsync(request.Numero, request.Nome, request.Pagina, request.Descricao,
            bytes, request.Imagem.ContentType, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Autor")]
    [RequestSizeLimit(4 * 1024 * 1024)]
    public async Task<FigurinhaDto> Atualizar(int id, [FromForm] FigurinhaFormRequest request, CancellationToken ct)
    {
        byte[]? bytes = null;
        string? contentType = null;
        if (request.Imagem is { Length: > 0 })
        {
            bytes = await LerAsync(request.Imagem, ct);
            contentType = request.Imagem.ContentType;
        }
        return await figurinhas.AtualizarAsync(id, request.Numero, request.Nome, request.Pagina, request.Descricao, bytes, contentType, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Autor")]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await figurinhas.RemoverAsync(id, ct);
        return NoContent();
    }

    /// <summary>Remove TODAS as figurinhas do álbum (PDF §8 — btnLimpar).</summary>
    [HttpPost("limpar")]
    [Authorize(Roles = "Autor")]
    public async Task<IActionResult> Limpar(CancellationToken ct)
    {
        await figurinhas.LimparTodasAsync(ct);
        return NoContent();
    }

    private static async Task<byte[]> LerAsync(IFormFile arquivo, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await arquivo.CopyToAsync(ms, ct);
        return ms.ToArray();
    }
}
