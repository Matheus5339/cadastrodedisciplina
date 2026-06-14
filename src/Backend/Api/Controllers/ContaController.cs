using AlbumFigurinhas.Api.Contracts;
using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Application.Features.Usuarios;
using AlbumFigurinhas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlbumFigurinhas.Api.Controllers;

/// <summary>Conta do próprio usuário autenticado: dados, troca de nome e de senha (PDF §7 / menu "Trocar senha").</summary>
[ApiController]
[Authorize]
[Route("api/conta")]
public sealed class ContaController(IUsuarioService usuarios, ICurrentUserService atual) : ControllerBase
{
    [HttpGet("me")]
    public Task<UsuarioDto> ObterMeusDados(CancellationToken ct) => usuarios.ObterAsync(atual.UsuarioId, ct);

    [HttpPut("me")]
    public Task<UsuarioDto> AtualizarMeusDados(AtualizarContaRequest request, CancellationToken ct) =>
        usuarios.AtualizarProprioAsync(request.Login, ct);

    [HttpPost("me/senha")]
    public async Task<IActionResult> TrocarSenha(TrocarSenhaRequest request, CancellationToken ct)
    {
        await usuarios.TrocarSenhaAsync(request.NovaSenha, ct);
        return NoContent();
    }
}
