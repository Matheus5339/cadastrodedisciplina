using System.Security.Claims;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ControleDisciplinas.Infrastructure.Auth;

/// <summary>
/// Identifica o aluno autenticado exclusivamente pelo claim "sub" do JWT
/// (regras de segurança 12/13 — nunca confiar em idAluno vindo do frontend).
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int AlunoId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var sub = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? user?.FindFirstValue("sub");
            if (sub is null || !int.TryParse(sub, out var id))
                throw new NaoAutorizadoException("Usuário não autenticado.");
            return id;
        }
    }
}
