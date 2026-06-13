using System.ComponentModel.DataAnnotations;

namespace ControleDisciplinas.Api.Contracts;

// Atributos nos parâmetros do construtor: exigência da validação de records no .NET 10.

public sealed record RegisterRequest(
    [Required, MaxLength(20)] string Rgu,
    [Required, MaxLength(14)] string Cpf,
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, MinLength(3), MaxLength(120)] string Nome,
    [Required, MinLength(8), MaxLength(128)] string Senha);

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Senha);

// Refresh e logout não recebem mais corpo: o refresh token vem do cookie httpOnly.

public sealed record UpdateAlunoRequest(
    [Required, MinLength(3), MaxLength(120)] string Nome,
    [Required, EmailAddress, MaxLength(254)] string Email);

public sealed record DisciplinaRequest(
    [Required, MaxLength(20)] string Codigo,
    [Required, MaxLength(150)] string Nome,
    [MaxLength(120)] string? Professor,
    [Range(1, 20)] int Periodo,
    [Range(0, 30)] int Creditos);

public sealed record HistoricoRequest(
    [Range(1, int.MaxValue)] int DisciplinaId,
    [Range(1980, 2100)] int Ano,
    [Range(1, 2)] int Semestre,
    [Range(1, 20)] int Periodo,
    [Range(0, 10)] decimal MediaFinal);
