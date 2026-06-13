using ControleDisciplinas.Api.Contracts;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Features.Auth;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ControleDisciplinas.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting(RateLimitPolicies.Auth)] // P0: anti brute-force nos endpoints de autenticação
public sealed class AuthController(IAuthService auth, IJwtTokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequest request, CancellationToken ct)
    {
        var resultado = await auth.RegistrarAsync(request.Rgu, request.Cpf, request.Email, request.Nome, request.Senha, ct);
        EmitirCookie(resultado.RefreshToken);
        return StatusCode(StatusCodes.Status201Created, AuthResponseDto.De(resultado));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequest request, CancellationToken ct)
    {
        var resultado = await auth.LoginAsync(request.Email, request.Senha, ct);
        EmitirCookie(resultado.RefreshToken);
        return AuthResponseDto.De(resultado);
    }

    /// <summary>Renova a sessão usando o refresh token do cookie httpOnly (sem corpo).</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie.Name];
        if (string.IsNullOrEmpty(refreshToken))
            throw new NaoAutorizadoException("Refresh token ausente.");

        var resultado = await auth.RefreshAsync(refreshToken, ct);
        EmitirCookie(resultado.RefreshToken);
        return AuthResponseDto.De(resultado);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie.Name];
        if (!string.IsNullOrEmpty(refreshToken))
            await auth.LogoutAsync(refreshToken, ct);

        RefreshTokenCookie.Limpar(Response, Request.IsHttps);
        return NoContent();
    }

    private void EmitirCookie(string refreshToken) =>
        RefreshTokenCookie.Definir(Response, refreshToken, tokens.RefreshTokenDias, Request.IsHttps);
}
