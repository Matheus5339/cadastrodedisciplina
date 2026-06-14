using ControleDisciplinas.Application.Features.Album;
using ControleDisciplinas.Application.Features.Arquivos;
using ControleDisciplinas.Application.Features.Auth;
using ControleDisciplinas.Application.Features.Colecao;
using ControleDisciplinas.Application.Features.Figurinhas;
using ControleDisciplinas.Application.Features.Usuarios;
using Microsoft.Extensions.DependencyInjection;

namespace ControleDisciplinas.Application;

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
