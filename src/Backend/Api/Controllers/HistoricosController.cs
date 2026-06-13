using ControleDisciplinas.Api.Contracts;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Features.Historicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDisciplinas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/historicos")]
public sealed class HistoricosController(IHistoricoService historicos) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HistoricoDto>>> Listar(
        [FromQuery] int? ano,
        [FromQuery] int? semestre,
        [FromQuery] string? disciplina,
        [FromQuery] string? professor,
        CancellationToken ct) =>
        Ok(await historicos.ListarAsync(ano, semestre, disciplina, professor, ct));

    [HttpGet("cr")]
    public async Task<ActionResult<CrDto>> CalcularCr(CancellationToken ct) =>
        await historicos.CalcularCrAsync(ct);

    [HttpPost]
    public async Task<ActionResult<HistoricoDto>> Criar(HistoricoRequest request, CancellationToken ct)
    {
        var dto = await historicos.CriarAsync(request.DisciplinaId, request.Ano, request.Semestre, request.Periodo, request.MediaFinal, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<HistoricoDto>> Atualizar(int id, HistoricoRequest request, CancellationToken ct) =>
        await historicos.AtualizarAsync(id, request.DisciplinaId, request.Ano, request.Semestre, request.Periodo, request.MediaFinal, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await historicos.RemoverAsync(id, ct);
        return NoContent();
    }
}
