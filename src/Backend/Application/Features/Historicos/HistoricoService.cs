using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;
using ControleDisciplinas.Domain.Services;

namespace ControleDisciplinas.Application.Features.Historicos;

public interface IHistoricoService
{
    Task<IReadOnlyList<HistoricoDto>> ListarAsync(int? ano, int? semestre, string? nomeDisciplina, string? professor, CancellationToken ct = default);
    Task<HistoricoDto> CriarAsync(int disciplinaId, int ano, int semestre, int periodo, decimal mediaFinal, CancellationToken ct = default);
    Task<HistoricoDto> AtualizarAsync(int id, int disciplinaId, int ano, int semestre, int periodo, decimal mediaFinal, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
    Task<CrDto> CalcularCrAsync(CancellationToken ct = default);
}

/// <summary>
/// Histórico 100% escopado ao aluno autenticado (regras de segurança 11–13):
/// o AlunoId vem sempre do token, nunca do corpo da requisição.
/// </summary>
public sealed class HistoricoService(
    IHistoricoRepository historicos,
    IDisciplinaRepository disciplinas,
    ICurrentUserService usuarioAtual,
    IUnitOfWork uow) : IHistoricoService
{
    public async Task<IReadOnlyList<HistoricoDto>> ListarAsync(int? ano, int? semestre, string? nomeDisciplina, string? professor, CancellationToken ct = default)
    {
        var filtro = new HistoricoFiltro(usuarioAtual.AlunoId, ano, semestre, nomeDisciplina, professor);
        var lista = await historicos.ListarAsync(filtro, ct);
        return lista.Select(h => h.ToDto()).ToList();
    }

    public async Task<HistoricoDto> CriarAsync(int disciplinaId, int ano, int semestre, int periodo, decimal mediaFinal, CancellationToken ct = default)
    {
        var alunoId = usuarioAtual.AlunoId;
        var disciplina = await disciplinas.ObterPorIdAsync(disciplinaId, ct)
            ?? throw new NaoEncontradoException("Disciplina não encontrada.");

        if (await historicos.ExisteLancamentoAsync(alunoId, disciplinaId, ano, semestre, ct: ct))
            throw new ConflitoException("Já existe lançamento desta disciplina neste ano/semestre.");

        var historico = new Historico(alunoId, disciplinaId, ano, semestre, periodo, mediaFinal);
        await historicos.AdicionarAsync(historico, ct);
        await uow.SaveChangesAsync(ct);

        return (await historicos.ObterPorIdAsync(historico.Id, alunoId, ct))!.ToDto();
    }

    public async Task<HistoricoDto> AtualizarAsync(int id, int disciplinaId, int ano, int semestre, int periodo, decimal mediaFinal, CancellationToken ct = default)
    {
        var alunoId = usuarioAtual.AlunoId;
        var historico = await historicos.ObterPorIdAsync(id, alunoId, ct)
            ?? throw new NaoEncontradoException("Lançamento de histórico não encontrado.");

        _ = await disciplinas.ObterPorIdAsync(disciplinaId, ct)
            ?? throw new NaoEncontradoException("Disciplina não encontrada.");

        if (await historicos.ExisteLancamentoAsync(alunoId, disciplinaId, ano, semestre, ignorarId: id, ct: ct))
            throw new ConflitoException("Já existe lançamento desta disciplina neste ano/semestre.");

        historico.TrocarDisciplina(disciplinaId);
        historico.Atualizar(ano, semestre, periodo, mediaFinal);
        await uow.SaveChangesAsync(ct);

        return (await historicos.ObterPorIdAsync(id, alunoId, ct))!.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var historico = await historicos.ObterPorIdAsync(id, usuarioAtual.AlunoId, ct)
            ?? throw new NaoEncontradoException("Lançamento de histórico não encontrado.");

        historicos.Remover(historico);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<CrDto> CalcularCrAsync(CancellationToken ct = default)
    {
        var notas = await historicos.ObterNotasComCreditosAsync(usuarioAtual.AlunoId, ct);
        var cr = CrCalculator.Calcular(notas);
        var totalCreditos = notas.Where(n => n.Creditos > 0).Sum(n => n.Creditos);
        return new CrDto(cr, totalCreditos, notas.Count);
    }
}
