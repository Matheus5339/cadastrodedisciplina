using ControleDisciplinas.Domain.Exceptions;

namespace ControleDisciplinas.Application.Validators;

/// <summary>Política mínima de senha (decisão D11 — validações nativas).</summary>
public static class SenhaValidator
{
    public const int TamanhoMinimo = 8;
    public const int TamanhoMaximo = 128;

    public static void Validar(string? senha)
    {
        if (string.IsNullOrEmpty(senha) || senha.Length < TamanhoMinimo || senha.Length > TamanhoMaximo)
            throw new ValidacaoException($"Senha deve ter entre {TamanhoMinimo} e {TamanhoMaximo} caracteres.");
        if (!senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
            throw new ValidacaoException("Senha deve conter ao menos uma letra e um número.");
    }
}
