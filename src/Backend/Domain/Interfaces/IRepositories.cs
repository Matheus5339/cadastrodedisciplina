using AlbumFigurinhas.Domain.Entities;

namespace AlbumFigurinhas.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken ct = default);
    Task<bool> ExisteLoginAsync(string login, int? ignorarId = null, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    /// <summary>Lista usuários, filtrando por parte do login digitado (PDF §6 — campo F).</summary>
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

/// <summary>
/// Projeção leve de figurinha para listagens: NÃO carrega a imagem (BLOB),
/// apenas a indicação <see cref="PossuiImagem"/>. Evita trafegar megabytes de
/// imagem do banco quando só os metadados são necessários.
/// </summary>
public sealed record FigurinhaResumo(
    int Id, int Numero, string Nome, int Pagina, string? Descricao, string Tag, bool PossuiImagem);

public interface IFigurinhaRepository
{
    Task<Figurinha?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Figurinha?> ObterPorTagAsync(string tag, CancellationToken ct = default);
    Task<bool> ExisteTagAsync(string tag, int? ignorarId = null, CancellationToken ct = default);
    Task<bool> ExisteNumeroAsync(int albumId, int numero, int? ignorarId = null, CancellationToken ct = default);
    /// <summary>Lista figurinhas COMPLETAS (inclui a imagem). Use só quando a imagem é necessária (ex.: exportação).</summary>
    Task<IReadOnlyList<Figurinha>> ListarAsync(int albumId, FigurinhaFiltro filtro, CancellationToken ct = default);
    /// <summary>Lista figurinhas SEM a imagem (projeção leve) — para telas/listagens de metadados.</summary>
    Task<IReadOnlyList<FigurinhaResumo>> ListarResumoAsync(int albumId, FigurinhaFiltro filtro, CancellationToken ct = default);
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
