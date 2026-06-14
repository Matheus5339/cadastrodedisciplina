using AlbumFigurinhas.Application.Interfaces;
using AlbumFigurinhas.Domain.Entities;
using AlbumFigurinhas.Domain.Enums;
using AlbumFigurinhas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlbumFigurinhas.Api.Extensions;

/// <summary>Dados iniciais: um usuário de cada perfil e o álbum único da aplicação.</summary>
public static class Seed
{
    public const string SenhaInicial = "123456";

    public static async Task ExecutarAsync(IServiceProvider sp, ILogger logger, CancellationToken ct = default)
    {
        var db = sp.GetRequiredService<AppDbContext>();
        var hasher = sp.GetRequiredService<IPasswordHasher>();

        if (!await db.Usuarios.AnyAsync(ct))
        {
            db.Usuarios.AddRange(
                new Usuario("admin", hasher.Hash(SenhaInicial), Perfil.Administrador),
                new Usuario("autor", hasher.Hash(SenhaInicial), Perfil.Autor),
                new Usuario("colecionador", hasher.Hash(SenhaInicial), Perfil.Colecionador));
            logger.LogInformation("Usuários iniciais criados (admin/autor/colecionador, senha {Senha}).", SenhaInicial);
        }

        if (!await db.Albuns.AnyAsync(ct))
        {
            db.Albuns.Add(new Album("Álbum FIFA 2014", 12));
            logger.LogInformation("Álbum inicial criado.");
        }

        await db.SaveChangesAsync(ct);
    }
}
