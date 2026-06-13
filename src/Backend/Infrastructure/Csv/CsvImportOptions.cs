namespace ControleDisciplinas.Infrastructure.Csv;

/// <summary>
/// Caminho do CSV é configurável (decisão D3). A divergência de nomes entre os
/// documentos ("discipolinas.csv" no normativo — typo; "disciplinas-ecomp-ucp.csv"
/// no arquivo real) é tratada pela lista de aliases pesquisada em ordem.
/// </summary>
public sealed class CsvImportOptions
{
    public const string Secao = "CsvImport";

    /// <summary>Caminho explícito do arquivo. Se definido, tem prioridade sobre Directory/FileNames.</summary>
    public string? Path { get; set; }

    /// <summary>Diretório onde procurar os arquivos (relativo ao content root ou absoluto).</summary>
    public string Directory { get; set; } = "data";

    /// <summary>Nomes pesquisados em ordem (padrão documentado + aliases).</summary>
    public string[] FileNames { get; set; } =
    [
        "disciplinas.csv",
        "discipolinas.csv",
        "disciplinas-ecomp-ucp.csv",
    ];
}
