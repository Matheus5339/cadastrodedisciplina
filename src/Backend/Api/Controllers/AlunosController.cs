using ControleDisciplinas.Api.Contracts;
using ControleDisciplinas.Api.Filters;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Features.Alunos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDisciplinas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/alunos")]
public sealed class AlunosController(IAlunoService alunos) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<AlunoDto>> ObterMeusDados(CancellationToken ct) =>
        await alunos.ObterMeusDadosAsync(ct);

    [HttpPut("me")]
    public async Task<ActionResult<AlunoDto>> AtualizarMeusDados(UpdateAlunoRequest request, CancellationToken ct) =>
        await alunos.AtualizarMeusDadosAsync(request.Nome, request.Email, ct);

    [HttpPut("me/foto")]
    [ValidarFoto]
    [RequestSizeLimit(4 * 1024 * 1024)] // margem acima do limite de 2 MB validado no domínio
    public async Task<IActionResult> EnviarFoto(IFormFile foto, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await foto.CopyToAsync(ms, ct);
        await alunos.DefinirFotoAsync(ms.ToArray(), foto.ContentType, ct);
        return NoContent();
    }

    [HttpGet("me/foto")]
    public async Task<IActionResult> ObterFoto(CancellationToken ct)
    {
        var fotoDto = await alunos.ObterFotoAsync(ct);
        return File(fotoDto.Conteudo, fotoDto.ContentType);
    }
}
