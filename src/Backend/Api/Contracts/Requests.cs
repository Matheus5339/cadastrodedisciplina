using System.ComponentModel.DataAnnotations;
using ControleDisciplinas.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ControleDisciplinas.Api.Contracts;

// Atributos diretamente nos parâmetros do construtor: exigência da validação de records no .NET 10.

public sealed record LoginRequest(
    [Required] string Nome,
    [Required] string Senha);

// Refresh e logout não recebem corpo: o refresh token vem do cookie httpOnly.

public sealed record CriarUsuarioRequest(
    [Required, MinLength(3), MaxLength(120)] string Nome,
    [Required, MinLength(8), MaxLength(128)] string Senha,
    [Required] Perfil Perfil);

public sealed record AtualizarUsuarioRequest(
    [Required, MinLength(3), MaxLength(120)] string Nome,
    [Required] Perfil Perfil);

public sealed record AtualizarContaRequest(
    [Required, MinLength(3), MaxLength(120)] string Nome);

public sealed record TrocarSenhaRequest(
    [Required, MinLength(8), MaxLength(128)] string NovaSenha);

public sealed record AtualizarAlbumRequest(
    [Required, MinLength(1), MaxLength(150)] string Nome,
    [Range(1, 1000)] int Paginas);

/// <summary>Dados da figurinha enviados como multipart/form-data (com a imagem).</summary>
public sealed class FigurinhaFormRequest
{
    [Range(1, 100000)] public int Numero { get; set; }
    [Required, MaxLength(150)] public string Nome { get; set; } = string.Empty;
    [Range(1, 1000)] public int Pagina { get; set; }
    [MaxLength(1000)] public string? Descricao { get; set; }
    public IFormFile? Imagem { get; set; }
}

public sealed record AdquirirRequest(
    [Required] string Tag);
