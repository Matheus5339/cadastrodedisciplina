namespace ControleDisciplinas.Domain.Services;

/// <summary>
/// Cálculo do Coeficiente de Rendimento (decisão D4, confirmada):
/// CR = soma(mediaFinal × creditos) / soma(creditos), arredondado a 2 casas.
/// Disciplinas com 0 créditos não entram no cálculo; sem créditos somados, CR é nulo.
/// </summary>
public static class CrCalculator
{
    public static decimal? Calcular(IEnumerable<(decimal MediaFinal, int Creditos)> itens)
    {
        decimal somaPonderada = 0;
        var somaCreditos = 0;

        foreach (var (mediaFinal, creditos) in itens)
        {
            if (creditos <= 0)
                continue;
            somaPonderada += mediaFinal * creditos;
            somaCreditos += creditos;
        }

        if (somaCreditos == 0)
            return null;

        return Math.Round(somaPonderada / somaCreditos, 2, MidpointRounding.AwayFromZero);
    }
}
