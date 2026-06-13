namespace ControleDisciplinas.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>Remove espaços nas pontas e converte vazio em null.</summary>
    public static string? TrimToNull(this string? valor)
    {
        var v = valor?.Trim();
        return string.IsNullOrEmpty(v) ? null : v;
    }

    /// <summary>Mantém apenas dígitos (útil para CPF).</summary>
    public static string ApenasDigitos(this string valor) =>
        new(valor.Where(char.IsDigit).ToArray());
}
