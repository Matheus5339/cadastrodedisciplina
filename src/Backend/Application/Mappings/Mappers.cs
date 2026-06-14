using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Domain.Entities;
using AlbumFigurinhas.Domain.Interfaces;

namespace AlbumFigurinhas.Application.Mappings;

public static class Mappers
{
    public static UsuarioDto ToDto(this Usuario u) =>
        new(u.Id, u.Login, u.Perfil);

    public static AlbumDto ToDto(this Album a) =>
        new(a.Id, a.Nome, a.Paginas, a.Capa is { Length: > 0 });

    public static FigurinhaDto ToDto(this Figurinha f) =>
        new(f.Id, f.Numero, f.Nome, f.Pagina, f.Descricao, f.Tag, f.Imagem is { Length: > 0 });

    public static FigurinhaAlbumDto ToAlbumDto(this Figurinha f, bool adquirida) =>
        new(f.Id, f.Numero, f.Nome, f.Pagina, f.Tag, f.Imagem is { Length: > 0 }, adquirida);

    // Projeções leves (sem a imagem) usadas nas listagens.
    public static FigurinhaDto ToDto(this FigurinhaResumo f) =>
        new(f.Id, f.Numero, f.Nome, f.Pagina, f.Descricao, f.Tag, f.PossuiImagem);

    public static FigurinhaAlbumDto ToAlbumDto(this FigurinhaResumo f, bool adquirida) =>
        new(f.Id, f.Numero, f.Nome, f.Pagina, f.Tag, f.PossuiImagem, adquirida);
}
