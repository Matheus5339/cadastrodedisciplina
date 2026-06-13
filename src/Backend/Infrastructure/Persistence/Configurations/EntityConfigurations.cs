using ControleDisciplinas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDisciplinas.Infrastructure.Persistence.Configurations;

public sealed class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> b)
    {
        b.ToTable("Alunos");
        b.HasKey(a => a.Id);
        b.Property(a => a.Rgu).HasMaxLength(20).IsRequired();
        b.Property(a => a.Cpf).HasMaxLength(11).IsRequired();
        b.Property(a => a.Email).HasMaxLength(254).IsRequired();
        b.Property(a => a.Nome).HasMaxLength(120).IsRequired();
        b.Property(a => a.PasswordHash).HasMaxLength(512).IsRequired();
        b.Property(a => a.Foto).HasColumnType("BLOB");
        b.Property(a => a.FotoContentType).HasMaxLength(50);

        b.HasIndex(a => a.Rgu).IsUnique();
        b.HasIndex(a => a.Cpf).IsUnique();
        b.HasIndex(a => a.Email).IsUnique();
    }
}

public sealed class DisciplinaConfiguration : IEntityTypeConfiguration<Disciplina>
{
    public void Configure(EntityTypeBuilder<Disciplina> b)
    {
        b.ToTable("Disciplinas");
        b.HasKey(d => d.Id);
        b.Property(d => d.Codigo).HasMaxLength(20).IsRequired();
        b.Property(d => d.Nome).HasMaxLength(150).IsRequired();
        b.Property(d => d.Professor).HasMaxLength(120);

        b.HasIndex(d => d.Codigo).IsUnique();
    }
}

public sealed class HistoricoConfiguration : IEntityTypeConfiguration<Historico>
{
    public void Configure(EntityTypeBuilder<Historico> b)
    {
        b.ToTable("Historicos");
        b.HasKey(h => h.Id);
        b.Property(h => h.MediaFinal).HasPrecision(4, 2);

        b.HasOne(h => h.Aluno)
            .WithMany()
            .HasForeignKey(h => h.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(h => h.Disciplina)
            .WithMany()
            .HasForeignKey(h => h.DisciplinaId)
            .OnDelete(DeleteBehavior.Restrict);

        // um lançamento por aluno/disciplina/ano/semestre
        b.HasIndex(h => new { h.AlunoId, h.DisciplinaId, h.Ano, h.Semestre }).IsUnique();
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

        b.HasOne(t => t.Aluno)
            .WithMany()
            .HasForeignKey(t => t.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(t => t.TokenHash).IsUnique();
        b.HasIndex(t => t.AlunoId);
    }
}

public sealed class ImportLogConfiguration : IEntityTypeConfiguration<ImportLog>
{
    public void Configure(EntityTypeBuilder<ImportLog> b)
    {
        b.ToTable("ImportLogs");
        b.HasKey(l => l.Id);
        b.Property(l => l.Arquivo).HasMaxLength(500).IsRequired();
        b.Property(l => l.HashArquivo).HasMaxLength(64).IsRequired();
    }
}
