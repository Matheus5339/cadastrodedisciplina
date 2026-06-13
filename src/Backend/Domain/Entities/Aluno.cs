using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.ValueObjects;
using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

public class Aluno : EntityBase
{
    public string Rgu { get; private set; } = null!;
    public string Cpf { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public byte[]? Foto { get; private set; }
    public string? FotoContentType { get; private set; }
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }

    private Aluno() { } // EF Core

    public Aluno(string rgu, string cpf, string email, string nome, string passwordHash)
    {
        Rgu = ValidarRgu(rgu);
        Cpf = ValueObjects.Cpf.Criar(cpf).Valor;
        Email = ValueObjects.Email.Criar(email).Valor;
        Nome = ValidarNome(nome);
        DefinirPasswordHash(passwordHash);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AtualizarPerfil(string nome, string email)
    {
        Nome = ValidarNome(nome);
        Email = ValueObjects.Email.Criar(email).Valor;
    }

    public void DefinirPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidacaoException("Hash de senha não pode ser vazio.");
        PasswordHash = passwordHash;
    }

    public void DefinirFoto(byte[] conteudo, string contentType, int tamanhoMaximoBytes)
    {
        if (conteudo is null || conteudo.Length == 0)
            throw new ValidacaoException("Foto vazia.");
        if (conteudo.Length > tamanhoMaximoBytes)
            throw new ValidacaoException($"Foto excede o limite de {tamanhoMaximoBytes / (1024 * 1024)} MB.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ValidacaoException("Tipo da foto não informado.");
        Foto = conteudo;
        FotoContentType = contentType;
    }

    public void RemoverFoto()
    {
        Foto = null;
        FotoContentType = null;
    }

    private static string ValidarRgu(string rgu)
    {
        var v = rgu?.Trim();
        if (string.IsNullOrEmpty(v) || v.Length > 20)
            throw new ValidacaoException("RGU é obrigatório (até 20 caracteres).");
        return v;
    }

    private static string ValidarNome(string nome)
    {
        var v = nome?.Trim();
        if (string.IsNullOrEmpty(v) || v.Length < 3 || v.Length > 120)
            throw new ValidacaoException("Nome é obrigatório (3 a 120 caracteres).");
        return v;
    }
}
