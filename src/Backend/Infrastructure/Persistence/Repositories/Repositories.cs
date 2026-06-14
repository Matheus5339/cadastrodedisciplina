using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleDisciplinas.Infrastructure.Persistence.Repositories;

public sealed class UsuarioRepository(AppDbContext db) : IUsuarioRepository
{
    public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken ct = default) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Login == login, ct);

    public Task<bool> ExisteLoginAsync(string login, int? ignorarId = null, CancellationToken ct = default) =>
        db.Usuarios.AnyAsync(u => u.Login == login && (ignorarId == null || u.Id != ignorarId), ct);

    public Task<int> ContarAsync(CancellationToken ct = default) => db.Usuarios.CountAsync(ct);

    public async Task<IReadOnlyList<Usuario>> ListarAsync(string? filtro, CancellationToken ct = default)
    {
        var query = db.Usuarios.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var f = $"%{filtro.Trim()}%";
            query = query.Where(u => EF.Functions.Like(u.Login, f));
        }
        return await query.OrderBy(u => u.Login).ToListAsync(ct);
    }

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default) =>
        await db.Usuarios.AddAsync(usuario, ct);

    public void Remover(Usuario usuario) => db.Usuarios.Remove(usuario);
}

public sealed class AlbumRepository(AppDbContext db) : IAlbumRepository
{
    public Task<Album?> ObterAsync(CancellationToken ct = default) =>
        db.Albuns.OrderBy(a => a.Id).FirstOrDefaultAsync(ct);

    public async Task AdicionarAsync(Album album, CancellationToken ct = default) =>
        await db.Albuns.AddAsync(album, ct);
}

public sealed class FigurinhaRepository(AppDbContext db) : IFigurinhaRepository
{
    public Task<Figurinha?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        db.Figurinhas.FirstOrDefaultAsync(f => f.Id == id, ct);

    public Task<Figurinha?> ObterPorTagAsync(string tag, CancellationToken ct = default) =>
        db.Figurinhas.FirstOrDefaultAsync(f => f.Tag == tag, ct);

    public Task<bool> ExisteTagAsync(string tag, int? ignorarId = null, CancellationToken ct = default) =>
        db.Figurinhas.AnyAsync(f => f.Tag == tag && (ignorarId == null || f.Id != ignorarId), ct);

    public Task<bool> ExisteNumeroAsync(int albumId, int numero, int? ignorarId = null, CancellationToken ct = default) =>
        db.Figurinhas.AnyAsync(f => f.AlbumId == albumId && f.Numero == numero && (ignorarId == null || f.Id != ignorarId), ct);

    public async Task<IReadOnlyList<Figurinha>> ListarAsync(int albumId, FigurinhaFiltro filtro, CancellationToken ct = default)
    {
        var query = db.Figurinhas.AsNoTracking().Where(f => f.AlbumId == albumId);

        if (!string.IsNullOrWhiteSpace(filtro.Texto))
        {
            var t = $"%{filtro.Texto.Trim()}%";
            query = query.Where(f => EF.Functions.Like(f.Nome, t) || EF.Functions.Like(f.Tag, t));
        }
        if (filtro.Pagina is int pagina)
            query = query.Where(f => f.Pagina == pagina);

        return await query.OrderBy(f => f.Numero).ToListAsync(ct);
    }

    public async Task AdicionarAsync(Figurinha figurinha, CancellationToken ct = default) =>
        await db.Figurinhas.AddAsync(figurinha, ct);

    public void Remover(Figurinha figurinha) => db.Figurinhas.Remove(figurinha);

    public async Task RemoverTodasAsync(int albumId, CancellationToken ct = default)
    {
        var todas = await db.Figurinhas.Where(f => f.AlbumId == albumId).ToListAsync(ct);
        db.Figurinhas.RemoveRange(todas);
    }
}

public sealed class FigurinhaAdquiridaRepository(AppDbContext db) : IFigurinhaAdquiridaRepository
{
    public Task<bool> ExisteAsync(int usuarioId, int figurinhaId, CancellationToken ct = default) =>
        db.FigurinhasAdquiridas.AnyAsync(a => a.UsuarioId == usuarioId && a.FigurinhaId == figurinhaId, ct);

    public async Task<IReadOnlyList<FigurinhaAdquirida>> ListarDoUsuarioAsync(int usuarioId, CancellationToken ct = default) =>
        await db.FigurinhasAdquiridas.AsNoTracking().Where(a => a.UsuarioId == usuarioId).ToListAsync(ct);

    public async Task AdicionarAsync(FigurinhaAdquirida adquirida, CancellationToken ct = default) =>
        await db.FigurinhasAdquiridas.AddAsync(adquirida, ct);
}

public sealed class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default) =>
        db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AdicionarAsync(RefreshToken token, CancellationToken ct = default) =>
        await db.RefreshTokens.AddAsync(token, ct);

    public async Task RevogarTodosDoUsuarioAsync(int usuarioId, CancellationToken ct = default)
    {
        var ativos = await db.RefreshTokens
            .Where(t => t.UsuarioId == usuarioId && t.RevokedAtUtc == null)
            .ToListAsync(ct);
        foreach (var token in ativos)
            token.Revogar();
    }
}
