using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Domain.Entities;

namespace ControleDisciplinas.Application.Mappings;

public static class Mappers
{
    public static UsuarioDto ToDto(this Usuario u) =>
        new(u.Id, u.Nome, u.Perfil);

    public static AlbumDto ToDto(this Album a) =>
        new(a.Id, a.Nome, a.Paginas, a.Capa is { Length: > 0 });

    public static FigurinhaDto ToDto(this Figurinha f) =>
        new(f.Id, f.Numero, f.Nome, f.Pagina, f.Descricao, f.Tag, f.Imagem is { Length: > 0 });

    public static FigurinhaAlbumDto ToAlbumDto(this Figurinha f, bool adquirida) =>
        new(f.Id, f.Numero, f.Nome, f.Pagina, f.Tag, f.Imagem is { Length: > 0 }, adquirida);
}
