namespace ControleDisciplinas.Shared.Constants;

public static class FotoConstants
{
    public const int TamanhoMaximoBytes = 2 * 1024 * 1024; // 2 MB

    public static readonly IReadOnlyDictionary<string, string> TiposPermitidos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
    };
}
