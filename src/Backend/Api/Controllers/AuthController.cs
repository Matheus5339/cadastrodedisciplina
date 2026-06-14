using AlbumFigurinhas.Api.Contracts;
using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Application.Features.Auth;
using AlbumFigurinhas.Application.Interfaces;
using AlbumFigurinhas.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AlbumFigurinhas.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting(RateLimitPolicies.Auth)] // anti brute-force
public sealed class AuthController(IAuthService auth, IJwtTokenService tokens) : ControllerBase
{
    /// <summary>Login por login + senha (PDF — FrmLogin).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequest request, CancellationToken ct)
    {
        var resultado = await auth.LoginAsync(request.Login, request.Senha, ct);
        EmitirCookie(resultado.RefreshToken);
        return AuthResponseDto.De(resultado);
    }

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
