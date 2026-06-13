using ControleDisciplinas.Api.Contracts;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Features.Disciplinas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDisciplinas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/disciplinas")]
public sealed class DisciplinasController(IDisciplinaService disciplinas) : ControllerBase
{
    /// <summary>Lista com filtros por nome, professor, semestre e/ou ano (requisito §2).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DisciplinaDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? professor,
        [FromQuery] int? ano,
        [FromQuery] int? semestre,
        CancellationToken ct) =>
        Ok(await disciplinas.ListarAsync(nome, professor, ano, semestre, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DisciplinaDto>> Obter(int id, CancellationToken ct) =>
        await disciplinas.ObterAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<DisciplinaDto>> Criar(DisciplinaRequest request, CancellationToken ct)
    {
        var dto = await disciplinas.CriarAsync(request.Codigo, request.Nome, request.Professor, request.Periodo, request.Creditos, ct);
        return CreatedAtAction(nameof(Obter), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DisciplinaDto>> Atualizar(int id, DisciplinaRequest request, CancellationToken ct) =>
        await disciplinas.AtualizarAsync(id, request.Codigo, request.Nome, request.Professor, request.Periodo, request.Creditos, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await disciplinas.RemoverAsync(id, ct);
        return NoContent();
    }
}
