using AlbumFigurinhas.Domain.Enums;
using AlbumFigurinhas.Domain.Exceptions;
using AlbumFigurinhas.Shared.Kernel;

namespace AlbumFigurinhas.Domain.Entities;

/// <summary>
/// Usuário do sistema com perfil de acesso. O login é feito pelo campo
/// <see cref="Login"/> + senha (PDF — FrmLogin/FrmUsuario usam "Login").
/// </summary>
public class Usuario : EntityBase
{
    public string Login { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Perfil Perfil { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Usuario() { } // EF Core

    public Usuario(string login, string passwordHash, Perfil perfil)
    {
        Login = ValidarLogin(login);
        DefinirPasswordHash(passwordHash);
        Perfil = perfil;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Edição completa pelo administrador (FrmUsuario): login e perfil.</summary>
    public void Atualizar(string login, Perfil perfil)
    {
        Login = ValidarLogin(login);
        Perfil = perfil;
    }

    /// <summary>Troca do próprio login — o perfil não muda (PDF §7).</summary>
    public void AtualizarProprioLogin(string login)
    {
        Login = ValidarLogin(login);
    }

    public void DefinirPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidacaoException("Hash de senha não pode ser vazio.");
        PasswordHash = passwordHash;
    }

    private static string ValidarLogin(string login)
    {
        var v = login?.Trim();
        if (string.IsNullOrEmpty(v) || v.Length < 3 || v.Length > 50)
            throw new ValidacaoException("Login é obrigatório (3 a 50 caracteres).");
        return v;
    }
}
