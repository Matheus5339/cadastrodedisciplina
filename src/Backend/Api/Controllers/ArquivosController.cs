using AlbumFigurinhas.Application.Features.Arquivos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlbumFigurinhas.Api.Controllers;

/// <summary>
/// Exportação/importação de figurinhas em arquivo TEXTO e BINÁRIO
/// (itens da rubrica: "arq texto" e "arq binario"). Acesso do Autor.
/// </summary>
[ApiController]
[Authorize(Roles = "Autor")]
[Route("api/arquivos")]
public sealed class ArquivosController(IArquivoService arquivos) : ControllerBase
{
    [HttpGet("figurinhas/texto")]
    public async Task<IActionResult> ExportarTexto(CancellationToken ct)
    {
        var conteudo = await arquivos.ExportarTextoAsync(ct);
        return File(System.Text.Encoding.UTF8.GetBytes(conteudo), "text/plain; charset=utf-8", "figurinhas.txt");
    }

    [HttpGet("figurinhas/binario")]
    public async Task<IActionResult> ExportarBinario(CancellationToken ct)
    {
        var bytes = await arquivos.ExportarBinarioAsync(ct);
        return File(bytes, "application/octet-stream", "figurinhas.figb");
    }

    [HttpPost("figurinhas/binario")]
    [RequestSizeLimit(32 * 1024 * 1024)]
    public async Task<ActionResult<object>> ImportarBinario(IFormFile arquivo, CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0)
            throw new Domain.Exceptions.ValidacaoException("Nenhum arquivo enviado (campo 'arquivo').");
        using var ms = new MemoryStream();
        await arquivo.CopyToAsync(ms, ct);
        var importadas = await arquivos.ImportarBinarioAsync(ms.ToArray(), ct);
        return Ok(new { importadas });
    }
}
