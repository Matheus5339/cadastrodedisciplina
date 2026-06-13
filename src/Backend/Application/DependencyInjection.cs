using ControleDisciplinas.Application.Features.Alunos;
using ControleDisciplinas.Application.Features.Auth;
using ControleDisciplinas.Application.Features.Disciplinas;
using ControleDisciplinas.Application.Features.Historicos;
using Microsoft.Extensions.DependencyInjection;

namespace ControleDisciplinas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAlunoService, AlunoService>();
        services.AddScoped<IDisciplinaService, DisciplinaService>();
        services.AddScoped<IHistoricoService, HistoricoService>();
        return services;
    }
}
