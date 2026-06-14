using AlbumFigurinhas.Application.Features.Album;
using AlbumFigurinhas.Application.Features.Arquivos;
using AlbumFigurinhas.Application.Features.Auth;
using AlbumFigurinhas.Application.Features.Colecao;
using AlbumFigurinhas.Application.Features.Figurinhas;
using AlbumFigurinhas.Application.Features.Usuarios;
using Microsoft.Extensions.DependencyInjection;

namespace AlbumFigurinhas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAlbumService, AlbumService>();
        services.AddScoped<IFigurinhaService, FigurinhaService>();
        services.AddScoped<IColecaoService, ColecaoService>();
        services.AddScoped<IArquivoService, ArquivoService>();
        return services;
    }
}
