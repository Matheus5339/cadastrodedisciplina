using System.Security.Claims;
using AlbumFigurinhas.Application.Interfaces;
using AlbumFigurinhas.Domain.Enums;
using AlbumFigurinhas.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AlbumFigurinhas.Infrastructure.Auth;

/// <summary>
/// Identifica o usuário autenticado exclusivamente pelos claims do JWT
/// (nunca confiar em id/perfil vindos do frontend).
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int UsuarioId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var sub = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("sub");
            if (sub is null || !int.TryParse(sub, out var id))
                throw new NaoAutorizadoException("Usuário não autenticado.");
            return id;
        }
    }

    public Perfil Perfil
    {
        get
        {
            var role = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            if (role is null || !Enum.TryParse<Perfil>(role, out var perfil))
                throw new NaoAutorizadoException("Perfil do usuário não identificado.");
            return perfil;
        }
    }
}
