using AlbumFigurinhas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlbumFigurinhas.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("Usuarios");
        b.HasKey(u => u.Id);
        b.Property(u => u.Login).HasMaxLength(50).IsRequired();
        b.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        b.Property(u => u.Perfil).HasConversion<int>();

        // o login é a credencial (PDF) — único
        b.HasIndex(u => u.Login).IsUnique();
    }
}

public sealed class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> b)
    {
        b.ToTable("Albuns");
        b.HasKey(a => a.Id);
        b.Property(a => a.Nome).HasMaxLength(150).IsRequired();
        b.Property(a => a.Capa).HasColumnType("BLOB");
        b.Property(a => a.CapaContentType).HasMaxLength(50);
    }
}

public sealed class FigurinhaConfiguration : IEntityTypeConfiguration<Figurinha>
{
    public void Configure(EntityTypeBuilder<Figurinha> b)
    {
        b.ToTable("Figurinhas");
        b.HasKey(f => f.Id);
        b.Property(f => f.Nome).HasMaxLength(150).IsRequired();
        b.Property(f => f.Descricao).HasMaxLength(1000);
        b.Property(f => f.Tag).HasMaxLength(64).IsRequired();
        b.Property(f => f.Imagem).HasColumnType("BLOB");
        b.Property(f => f.ImagemContentType).HasMaxLength(50);

        b.HasOne(f => f.Album)
            .WithMany()
            .HasForeignKey(f => f.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(f => f.Tag).IsUnique();
        b.HasIndex(f => new { f.AlbumId, f.Numero }).IsUnique();
    }
}

public sealed class FigurinhaAdquiridaConfiguration : IEntityTypeConfiguration<FigurinhaAdquirida>
{
    public void Configure(EntityTypeBuilder<FigurinhaAdquirida> b)
    {
        b.ToTable("FigurinhasAdquiridas");
        b.HasKey(a => a.Id);

        b.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(a => a.Figurinha)
            .WithMany()
            .HasForeignKey(a => a.FigurinhaId)
            .OnDelete(DeleteBehavior.Cascade);

        // um colecionador não adquire a mesma figurinha duas vezes
        b.HasIndex(a => new { a.UsuarioId, a.FigurinhaId }).IsUnique();
    }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");
        b.HasKey(t => t.Id);
        b.Property(t => t.TokenHash).HasMaxLength(128).IsRequired();
        b.Property(t => t.ReplacedByTokenHash).HasMaxLength(128);

        b.HasOne(t => t.Usuario)
            .WithMany()
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(t => t.TokenHash).IsUnique();
        b.HasIndex(t => t.UsuarioId);
    }
}
