using ControleDisciplinas.Domain.Entities;

namespace ControleDisciplinas.Domain.Interfaces;

public interface IAlunoRepository
{
    Task<Aluno?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Aluno?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExisteEmailAsync(string email, int? ignorarId = null, CancellationToken ct = default);
    Task<bool> ExisteCpfAsync(string cpf, int? ignorarId = null, CancellationToken ct = default);
    Task<bool> ExisteRguAsync(string rgu, int? ignorarId = null, CancellationToken ct = default);
    Task AdicionarAsync(Aluno aluno, CancellationToken ct = default);
}

public sealed record DisciplinaFiltro(string? Nome, string? Professor, int? Ano, int? Semestre, int? AlunoIdParaAnoSemestre);

public interface IDisciplinaRepository
{
    Task<Disciplina?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Disciplina?> ObterPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task<bool> ExisteCodigoAsync(string codigo, int? ignorarId = null, CancellationToken ct = default);
    Task<IReadOnlyList<Disciplina>> ListarAsync(DisciplinaFiltro filtro, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ListarCodigosAsync(CancellationToken ct = default);
    Task<bool> PossuiHistoricoAsync(int disciplinaId, CancellationToken ct = default);
    Task AdicionarAsync(Disciplina disciplina, CancellationToken ct = default);
    void Remover(Disciplina disciplina);
}

public sealed record HistoricoFiltro(int AlunoId, int? Ano, int? Semestre, string? NomeDisciplina, string? Professor);

public interface IHistoricoRepository
{
    Task<Historico?> ObterPorIdAsync(int id, int alunoId, CancellationToken ct = default);
    Task<IReadOnlyList<Historico>> ListarAsync(HistoricoFiltro filtro, CancellationToken ct = default);
    Task<bool> ExisteLancamentoAsync(int alunoId, int disciplinaId, int ano, int semestre, int? ignorarId = null, CancellationToken ct = default);
    Task<IReadOnlyList<(decimal MediaFinal, int Creditos)>> ObterNotasComCreditosAsync(int alunoId, CancellationToken ct = default);
    Task AdicionarAsync(Historico historico, CancellationToken ct = default);
    void Remover(Historico historico);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default);
    Task AdicionarAsync(RefreshToken token, CancellationToken ct = default);
    Task RevogarTodosDoAlunoAsync(int alunoId, CancellationToken ct = default);
}

public interface IImportLogRepository
{
    Task AdicionarAsync(ImportLog log, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
