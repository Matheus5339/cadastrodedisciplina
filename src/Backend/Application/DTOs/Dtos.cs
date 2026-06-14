using ControleDisciplinas.Domain.Enums;

namespace ControleDisciplinas.Application.DTOs;

/// <summary>Resultado interno da autenticação (inclui o refresh token bruto para o cookie).</summary>
public sealed record AuthResultDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    UsuarioDto Usuario);

/// <summary>Corpo retornado ao cliente: sem o refresh token (que viaja em cookie httpOnly).</summary>
public sealed record AuthResponseDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    UsuarioDto Usuario)
{
    public static AuthResponseDto De(AuthResultDto r) => new(r.AccessToken, r.AccessTokenExpiresAtUtc, r.Usuario);
}

public sealed record UsuarioDto(int Id, string Login, Perfil Perfil);

public sealed record AlbumDto(int Id, string Nome, int Paginas, bool PossuiCapa);

public sealed record FigurinhaDto(
    int Id,
    int Numero,
    string Nome,
    int Pagina,
    string? Descricao,
    string Tag,
    bool PossuiImagem);

/// <summary>Figurinha na visão do colecionador, com a marca de adquirida ou não.</summary>
public sealed record FigurinhaAlbumDto(
    int Id,
    int Numero,
    string Nome,
    int Pagina,
    string Tag,
    bool PossuiImagem,
    bool Adquirida);

/// <summary>Visão do álbum para o colecionador: álbum + figurinhas por página.</summary>
public sealed record AlbumColecionadorDto(AlbumDto Album, IReadOnlyList<FigurinhaAlbumDto> Figurinhas);

public sealed record ImagemDto(byte[] Conteudo, string ContentType);
