using System.Text.RegularExpressions;
using ControleDisciplinas.Domain.Exceptions;

namespace ControleDisciplinas.Domain.ValueObjects;

/// <summary>E-mail validado e normalizado (minúsculas, sem espaços).</summary>
public sealed partial record Email
{
    public string Valor { get; }

    private Email(string valor) => Valor = valor;

    public static Email Criar(string? valor)
    {
        var v = valor?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(v) || v.Length > 254 || !Padrao().IsMatch(v))
            throw new ValidacaoException("E-mail inválido.");
        return new Email(v);
    }

    public override string ToString() => Valor;

    [GeneratedRegex(@"^[a-z0-9._%+-]+@[a-z0-9-]+(\.[a-z0-9-]+)+$")]
    private static partial Regex Padrao();
}
