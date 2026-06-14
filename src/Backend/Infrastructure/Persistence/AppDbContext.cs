using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleDisciplinas.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Album> Albuns => Set<Album>();
    public DbSet<Figurinha> Figurinhas => Set<Figurinha>();
    public DbSet<FigurinhaAdquirida> FigurinhasAdquiridas => Set<FigurinhaAdquirida>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
