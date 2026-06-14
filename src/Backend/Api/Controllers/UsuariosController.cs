using ControleDisciplinas.Api.Contracts;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Features.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDisciplinas.Api.Controllers;

/// <summary>Gestão de usuários — acesso exclusivo do Administrador (PDF §6/§7).</summary>
[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/usuarios")]
public sealed class UsuariosController(IUsuarioService usuarios) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> Listar([FromQuery] string? filtro, CancellationToken ct) =>
        Ok(await usuarios.ListarAsync(filtro, ct));

    [HttpGet("{id:int}")]
    public Task<UsuarioDto> Obter(int id, CancellationToken ct) => usuarios.ObterAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Criar(CriarUsuarioRequest request, CancellationToken ct)
    {
        var dto = await usuarios.CriarAsync(request.Nome, request.Senha, request.Perfil, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpPut("{id:int}")]
    public Task<UsuarioDto> Atualizar(int id, AtualizarUsuarioRequest request, CancellationToken ct) =>
        usuarios.AtualizarAsync(id, request.Nome, request.Perfil, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await usuarios.RemoverAsync(id, ct);
        return NoContent();
    }

    /// <summary>Zera a senha do usuário, devolvendo a senha padrão gerada (assignment §adm).</summary>
    [HttpPost("{id:int}/resetar-senha")]
    public async Task<ActionResult<object>> ResetarSenha(int id, CancellationToken ct)
    {
        var senha = await usuarios.ResetarSenhaAsync(id, ct);
        return Ok(new { senha });
    }
}
