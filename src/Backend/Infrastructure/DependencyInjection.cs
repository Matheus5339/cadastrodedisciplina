using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Interfaces;
using ControleDisciplinas.Infrastructure.Auth;
using ControleDisciplinas.Infrastructure.Persistence;
using ControleDisciplinas.Infrastructure.Persistence.Repositories;
using ControleDisciplinas.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleDisciplinas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=data/album-figurinhas.db";

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IFigurinhaRepository, FigurinhaRepository>();
        services.AddScoped<IFigurinhaAdquiridaRepository, FigurinhaAdquiridaRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.Configure<PasswordHashOptions>(configuration.GetSection(PasswordHashOptions.Secao));
        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Secao));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
