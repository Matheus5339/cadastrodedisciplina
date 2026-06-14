namespace AlbumFigurinhas.Shared.Kernel;

/// <summary>Resultado paginado genérico para listagens.</summary>
public sealed record PaginatedResult<T>(IReadOnlyList<T> Itens, int Total, int Pagina, int TamanhoPagina)
{
    public int TotalPaginas => TamanhoPagina <= 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanhoPagina);
}
