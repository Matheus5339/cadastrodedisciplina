using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;

namespace ControleDisciplinas.Application.Features.Disciplinas;

public interface IDisciplinaService
{
    Task<IReadOnlyList<DisciplinaDto>> ListarAsync(string? nome, string? professor, int? ano, int? semestre, CancellationToken ct = default);
    Task<DisciplinaDto> ObterAsync(int id, CancellationToken ct = default);
    Task<DisciplinaDto> CriarAsync(string codigo, string nome, string? professor, int periodo, int creditos, CancellationToken ct = default);
    Task<DisciplinaDto> AtualizarAsync(int id, string codigo, string nome, string? professor, int periodo, int creditos, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

/// <summary>
/// Catálogo global de disciplinas (decisão D10). Os filtros de ano/semestre
/// cruzam com o histórico do aluno autenticado (disciplinas cursadas naquele ano/semestre).
/// </summary>
public sealed class DisciplinaService(
    IDisciplinaRepository disciplinas,
    ICurrentUserService usuarioAtual,
    IUnitOfWork uow) : IDisciplinaService
{
    public async Task<IReadOnlyList<DisciplinaDto>> ListarAsync(string? nome, string? professor, int? ano, int? semestre, CancellationToken ct = default)
    {
        var alunoIdParaAnoSemestre = (ano.HasValue || semestre.HasValue) ? usuarioAtual.AlunoId : (int?)null;
        var filtro = new DisciplinaFiltro(nome, professor, ano, semestre, alunoIdParaAnoSemestre);
        var lista = await disciplinas.ListarAsync(filtro, ct);
        return lista.Select(d => d.ToDto()).ToList();
    }

    public async Task<DisciplinaDto> ObterAsync(int id, CancellationToken ct = default) =>
        (await ObterEntidadeAsync(id, ct)).ToDto();

    public async Task<DisciplinaDto> CriarAsync(string codigo, string nome, string? professor, int periodo, int creditos, CancellationToken ct = default)
    {
        var disciplina = new Disciplina(codigo, nome, professor, periodo, creditos);
        if (await disciplinas.ExisteCodigoAsync(disciplina.Codigo, ct: ct))
            throw new ConflitoException($"Já existe disciplina com o código {disciplina.Codigo}.");

        await disciplinas.AdicionarAsync(disciplina, ct);
        await uow.SaveChangesAsync(ct);
        return disciplina.ToDto();
    }

    public async Task<DisciplinaDto> AtualizarAsync(int id, string codigo, string nome, string? professor, int periodo, int creditos, CancellationToken ct = default)
    {
        var disciplina = await ObterEntidadeAsync(id, ct);
        disciplina.Atualizar(codigo, nome, professor, periodo, creditos);
        if (await disciplinas.ExisteCodigoAsync(disciplina.Codigo, ignorarId: id, ct: ct))
            throw new ConflitoException($"Já existe outra disciplina com o código {disciplina.Codigo}.");

        await uow.SaveChangesAsync(ct);
        return disciplina.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var disciplina = await ObterEntidadeAsync(id, ct);
        if (await disciplinas.PossuiHistoricoAsync(id, ct))
            throw new ConflitoException("Disciplina possui lançamentos de histórico e não pode ser removida.");

        disciplinas.Remover(disciplina);
        await uow.SaveChangesAsync(ct);
    }

    private async Task<Disciplina> ObterEntidadeAsync(int id, CancellationToken ct) =>
        await disciplinas.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException("Disciplina não encontrada.");
}
