using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

/// <summary>
/// Tabela técnica (decisão D7): registro de cada execução da importação de CSV
/// na inicialização, para auditoria e idempotência.
/// </summary>
public class ImportLog : EntityBase
{
    public string Arquivo { get; private set; } = null!;
    public string HashArquivo { get; private set; } = null!;
    public DateTime ImportadoEmUtc { get; private set; }
    public int Importados { get; private set; }
    public int Ignorados { get; private set; }
    public int LinhasInvalidas { get; private set; }

    private ImportLog() { } // EF Core

    public ImportLog(string arquivo, string hashArquivo, int importados, int ignorados, int linhasInvalidas)
    {
        Arquivo = arquivo;
        HashArquivo = hashArquivo;
        ImportadoEmUtc = DateTime.UtcNow;
        Importados = importados;
        Ignorados = ignorados;
        LinhasInvalidas = linhasInvalidas;
    }
}
