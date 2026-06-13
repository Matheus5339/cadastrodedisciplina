using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleDisciplinas.Infrastructure.Persistence.Repositories;

public sealed class AlunoRepository(AppDbContext db) : IAlunoRepository
{
    public Task<Aluno?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        db.Alunos.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Aluno?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        db.Alunos.FirstOrDefaultAsync(a => a.Email == email, ct);

    public Task<bool> ExisteEmailAsync(string email, int? ignorarId = null, CancellationToken ct = default) =>
        db.Alunos.AnyAsync(a => a.Email == email && (ignorarId == null || a.Id != ignorarId), ct);

    public Task<bool> ExisteCpfAsync(string cpf, int? ignorarId = null, CancellationToken ct = default) =>
        db.Alunos.AnyAsync(a => a.Cpf == cpf && (ignorarId == null || a.Id != ignorarId), ct);

    public Task<bool> ExisteRguAsync(string rgu, int? ignorarId = null, CancellationToken ct = default) =>
        db.Alunos.AnyAsync(a => a.Rgu == rgu && (ignorarId == null || a.Id != ignorarId), ct);

    public async Task AdicionarAsync(Aluno aluno, CancellationToken ct = default) =>
        await db.Alunos.AddAsync(aluno, ct);
}

public sealed class DisciplinaRepository(AppDbContext db) : IDisciplinaRepository
{
    public Task<Disciplina?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        db.Disciplinas.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<Disciplina?> ObterPorCodigoAsync(string codigo, CancellationToken ct = default) =>
        db.Disciplinas.FirstOrDefaultAsync(d => d.Codigo == codigo, ct);

    public Task<bool> ExisteCodigoAsync(string codigo, int? ignorarId = null, CancellationToken ct = default) =>
        db.Disciplinas.AnyAsync(d => d.Codigo == codigo && (ignorarId == null || d.Id != ignorarId), ct);

    public async Task<IReadOnlyList<Disciplina>> ListarAsync(DisciplinaFiltro filtro, CancellationToken ct = default)
    {
        var query = db.Disciplinas.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = $"%{filtro.Nome.Trim()}%";
            query = query.Where(d => EF.Functions.Like(d.Nome, nome) || EF.Functions.Like(d.Codigo, nome));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Professor))
        {
            var prof = $"%{filtro.Professor.Trim()}%";
            query = query.Where(d => d.Professor != null && EF.Functions.Like(d.Professor, prof));
        }

        // ano/semestre filtram pelas disciplinas cursadas pelo aluno autenticado (decisão D10)
        if (filtro.AlunoIdParaAnoSemestre is int alunoId)
        {
            query = query.Where(d => db.Historicos.Any(h =>
                h.DisciplinaId == d.Id &&
                h.AlunoId == alunoId &&
                (filtro.Ano == null || h.Ano == filtro.Ano) &&
                (filtro.Semestre == null || h.Semestre == filtro.Semestre)));
        }

        return await query.OrderBy(d => d.Periodo).ThenBy(d => d.Codigo).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> ListarCodigosAsync(CancellationToken ct = default) =>
        await db.Disciplinas.AsNoTracking().Select(d => d.Codigo).ToListAsync(ct);

    public Task<bool> PossuiHistoricoAsync(int disciplinaId, CancellationToken ct = default) =>
        db.Historicos.AnyAsync(h => h.DisciplinaId == disciplinaId, ct);

    public async Task AdicionarAsync(Disciplina disciplina, CancellationToken ct = default) =>
        await db.Disciplinas.AddAsync(disciplina, ct);

    public void Remover(Disciplina disciplina) => db.Disciplinas.Remove(disciplina);
}

public sealed class HistoricoRepository(AppDbContext db) : IHistoricoRepository
{
    public Task<Historico?> ObterPorIdAsync(int id, int alunoId, CancellationToken ct = default) =>
        db.Historicos.Include(h => h.Disciplina)
            .FirstOrDefaultAsync(h => h.Id == id && h.AlunoId == alunoId, ct);

    public async Task<IReadOnlyList<Historico>> ListarAsync(HistoricoFiltro filtro, CancellationToken ct = default)
    {
        var query = db.Historicos.AsNoTracking()
            .Include(h => h.Disciplina)
            .Where(h => h.AlunoId == filtro.AlunoId);

        if (filtro.Ano is not null)
            query = query.Where(h => h.Ano == filtro.Ano);
        if (filtro.Semestre is not null)
            query = query.Where(h => h.Semestre == filtro.Semestre);
        if (!string.IsNullOrWhiteSpace(filtro.NomeDisciplina))
        {
            var nome = $"%{filtro.NomeDisciplina.Trim()}%";
            query = query.Where(h => EF.Functions.Like(h.Disciplina!.Nome, nome) || EF.Functions.Like(h.Disciplina!.Codigo, nome));
        }
        if (!string.IsNullOrWhiteSpace(filtro.Professor))
        {
            var prof = $"%{filtro.Professor.Trim()}%";
            query = query.Where(h => h.Disciplina!.Professor != null && EF.Functions.Like(h.Disciplina!.Professor, prof));
        }

        return await query
            .OrderByDescending(h => h.Ano).ThenByDescending(h => h.Semestre).ThenBy(h => h.Disciplina!.Nome)
            .ToListAsync(ct);
    }

    public Task<bool> ExisteLancamentoAsync(int alunoId, int disciplinaId, int ano, int semestre, int? ignorarId = null, CancellationToken ct = default) =>
        db.Historicos.AnyAsync(h =>
            h.AlunoId == alunoId && h.DisciplinaId == disciplinaId && h.Ano == ano && h.Semestre == semestre &&
            (ignorarId == null || h.Id != ignorarId), ct);

    public async Task<IReadOnlyList<(decimal MediaFinal, int Creditos)>> ObterNotasComCreditosAsync(int alunoId, CancellationToken ct = default)
    {
        var linhas = await db.Historicos.AsNoTracking()
            .Where(h => h.AlunoId == alunoId)
            .Select(h => new { h.MediaFinal, h.Disciplina!.Creditos })
            .ToListAsync(ct);
        return linhas.Select(l => (l.MediaFinal, l.Creditos)).ToList();
    }

    public async Task AdicionarAsync(Historico historico, CancellationToken ct = default) =>
        await db.Historicos.AddAsync(historico, ct);

    public void Remover(Historico historico) => db.Historicos.Remove(historico);
}

public sealed class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default) =>
        db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AdicionarAsync(RefreshToken token, CancellationToken ct = default) =>
        await db.RefreshTokens.AddAsync(token, ct);

    public async Task RevogarTodosDoAlunoAsync(int alunoId, CancellationToken ct = default)
    {
        var ativos = await db.RefreshTokens
            .Where(t => t.AlunoId == alunoId && t.RevokedAtUtc == null)
            .ToListAsync(ct);
        foreach (var token in ativos)
            token.Revogar();
    }
}

public sealed class ImportLogRepository(AppDbContext db) : IImportLogRepository
{
    public async Task AdicionarAsync(ImportLog log, CancellationToken ct = default) =>
        await db.ImportLogs.AddAsync(log, ct);
}
