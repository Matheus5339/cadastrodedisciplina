namespace ControleDisciplinas.Shared.Helpers;

/// <summary>
/// Parser de linha CSV (separador vírgula) com suporte a campos entre aspas
/// duplas, incluindo vírgulas e aspas escapadas ("") dentro do campo.
/// </summary>
public static class CsvLineParser
{
    public static IReadOnlyList<string> Parse(string linha)
    {
        var campos = new List<string>();
        var atual = new System.Text.StringBuilder();
        var dentroDeAspas = false;

        for (var i = 0; i < linha.Length; i++)
        {
            var c = linha[i];
            if (dentroDeAspas)
            {
                if (c == '"')
                {
                    if (i + 1 < linha.Length && linha[i + 1] == '"')
                    {
                        atual.Append('"');
                        i++;
                    }
                    else
                    {
                        dentroDeAspas = false;
                    }
                }
                else
                {
                    atual.Append(c);
                }
            }
            else if (c == '"')
            {
                dentroDeAspas = true;
            }
            else if (c == ',')
            {
                campos.Add(atual.ToString());
                atual.Clear();
            }
            else
            {
                atual.Append(c);
            }
        }

        campos.Add(atual.ToString());
        return campos;
    }
}
