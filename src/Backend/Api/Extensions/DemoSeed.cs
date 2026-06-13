using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleDisciplinas.Api.Extensions;

/// <summary>
/// Cria o aluno de demonstração para avaliação acadêmica quando
/// Seed:DemoAluno = true (somente fora de produção). Credenciais documentadas
/// em docs/ENTREGA.md. A senha vem da configuração, nunca de código.
/// </summary>
public static class DemoSeed
{
    public static async Task ExecutarAsync(IServiceProvider services, IConfiguration configuration, IHostEnvironment env, ILogger logger)
    {
        if (env.IsProduction() || !configuration.GetValue<bool>("Seed:DemoAluno"))
            return;

        var senha = configuration["Seed:DemoSenha"];
        if (string.IsNullOrWhiteSpace(senha))
        {
            logger.LogWarning("Seed:DemoAluno habilitado, mas Seed:DemoSenha não configurada — seed ignorado.");
            return;
        }

        var db = services.GetRequiredService<AppDbContext>();
        const string email = "demo@ucp.edu.br";
        if (await db.Alunos.AnyAsync(a => a.Email == email))
            return;

        var hasher = services.GetRequiredService<IPasswordHasher>();
        // CPF válido gerado para demonstração (não pertence a pessoa real)
        var demo = new Aluno("2026100001", "52998224725", email, "Aluno de Demonstração", hasher.Hash(senha));
        db.Alunos.Add(demo);
        await db.SaveChangesAsync();
        logger.LogInformation("Aluno de demonstração criado ({Email}).", email);
    }
}
