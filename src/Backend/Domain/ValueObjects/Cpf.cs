using ControleDisciplinas.Domain.Exceptions;

namespace ControleDisciplinas.Domain.ValueObjects;

/// <summary>CPF validado pelos dígitos verificadores, armazenado só com dígitos.</summary>
public sealed record Cpf
{
    public string Valor { get; }

    private Cpf(string valor) => Valor = valor;

    public static Cpf Criar(string? valor)
    {
        var digitos = new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        if (!EhValido(digitos))
            throw new ValidacaoException("CPF inválido.");
        return new Cpf(digitos);
    }

    public static bool EhValido(string digitos)
    {
        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
            return false;

        var numeros = digitos.Select(c => c - '0').ToArray();
        for (var dv = 9; dv < 11; dv++)
        {
            var soma = 0;
            for (var i = 0; i < dv; i++)
                soma += numeros[i] * (dv + 1 - i);
            var resto = soma * 10 % 11 % 10;
            if (numeros[dv] != resto)
                return false;
        }
        return true;
    }

    public override string ToString() => Valor;
}
