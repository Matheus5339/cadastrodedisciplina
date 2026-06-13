namespace ControleDisciplinas.Infrastructure.Security;

/// <summary>Parâmetros do Argon2id. O Pepper, se usado, vem de configuração não versionada.</summary>
public sealed class PasswordHashOptions
{
    public const string Secao = "PasswordHash";

    /// <summary>Custo de memória em KiB (padrão 64 MiB).</summary>
    public int MemoryCostKib { get; set; } = 65536;

    /// <summary>Iterações.</summary>
    public int TimeCost { get; set; } = 3;

    /// <summary>Paralelismo.</summary>
    public int Lanes { get; set; } = 2;

    /// <summary>Tamanho do hash em bytes.</summary>
    public int HashLength { get; set; } = 32;

    /// <summary>Segredo adicional (opcional). Nunca versionar.</summary>
    public string? Pepper { get; set; }
}
