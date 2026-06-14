using ControleDisciplinas.Domain.Entities;

namespace ControleDisciplinas.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> ObterPorNomeAsync(string nome, CancellationToken ct = default);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    /// <summary>Lista usuários, filtrando por parte do nome digitado (PDF §6 — campo F).</summary>
    Task<IReadOnlyList<Usuario>> ListarAsync(string? filtro, CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
    void Remover(Usuario usuario);
}

public interface IAlbumRepository
{
    /// <summary>Obtém o álbum único da aplicação (ou null se ainda não existe).</summary>
    Task<Album?> ObterAsync(CancellationToken ct = default);
    Task AdicionarAsync(Album album, CancellationToken ct = default);
}

public sealed record FigurinhaFiltro(string? Texto, int? Pagina);

public interface IFigurinhaRepository
{
    Task<Figurinha?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Figurinha?> ObterPorTagAsync(string tag, CancellationToken ct = default);
    Task<bool> ExisteTagAsync(string tag, int? ignorarId = null, CancellationToken ct = default);
    Task<bool> ExisteNumeroAsync(int albumId, int numero, int? ignorarId = null, CancellationToken ct = default);
    Task<IReadOnlyList<Figurinha>> ListarAsync(int albumId, FigurinhaFiltro filtro, CancellationToken ct = default);
    Task AdicionarAsync(Figurinha figurinha, CancellationToken ct = default);
    void Remover(Figurinha figurinha);
    Task RemoverTodasAsync(int albumId, CancellationToken ct = default);
}

public interface IFigurinhaAdquiridaRepository
{
    Task<bool> ExisteAsync(int usuarioId, int figurinhaId, CancellationToken ct = default);
    Task<IReadOnlyList<FigurinhaAdquirida>> ListarDoUsuarioAsync(int usuarioId, CancellationToken ct = default);
    Task AdicionarAsync(FigurinhaAdquirida adquirida, CancellationToken ct = default);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default);
    Task AdicionarAsync(RefreshToken token, CancellationToken ct = default);
    Task RevogarTodosDoUsuarioAsync(int usuarioId, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
