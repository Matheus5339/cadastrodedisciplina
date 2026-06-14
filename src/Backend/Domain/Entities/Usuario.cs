using ControleDisciplinas.Domain.Enums;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

/// <summary>
/// Usuário do sistema com perfil de acesso. O login é feito pelo <see cref="Nome"/>
/// + senha (assignment §login: "a tela de login deverá pedir o nome e a senha").
/// </summary>
public class Usuario : EntityBase
{
    public string Nome { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Perfil Perfil { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Usuario() { } // EF Core

    public Usuario(string nome, string passwordHash, Perfil perfil)
    {
        Nome = ValidarNome(nome);
        DefinirPasswordHash(passwordHash);
        Perfil = perfil;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Edição completa pelo administrador (FrmUsuario): nome e perfil.</summary>
    public void Atualizar(string nome, Perfil perfil)
    {
        Nome = ValidarNome(nome);
        Perfil = perfil;
    }

    /// <summary>Troca do próprio nome — o perfil não muda (PDF §7).</summary>
    public void AtualizarProprioNome(string nome)
    {
        Nome = ValidarNome(nome);
    }

    public void DefinirPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidacaoException("Hash de senha não pode ser vazio.");
        PasswordHash = passwordHash;
    }

    private static string ValidarNome(string nome)
    {
        var v = nome?.Trim();
        if (string.IsNullOrEmpty(v) || v.Length < 3 || v.Length > 120)
            throw new ValidacaoException("Nome é obrigatório (3 a 120 caracteres).");
        return v;
    }
}
