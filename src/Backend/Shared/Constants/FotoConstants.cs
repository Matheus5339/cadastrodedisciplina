namespace AlbumFigurinhas.Shared.Constants;

public static class FotoConstants
{
    public const int TamanhoMaximoBytes = 2 * 1024 * 1024; // 2 MB

    public static readonly IReadOnlyDictionary<string, string> TiposPermitidos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
    };

    /// <summary>
    /// Detecta o tipo da imagem pela ASSINATURA (magic numbers) dos primeiros bytes,
    /// independentemente do Content-Type declarado pelo cliente. Retorna o content-type
    /// (image/png|jpeg|webp) ou <c>null</c> se não for uma imagem suportada.
    /// </summary>
    public static string? DetectarContentType(ReadOnlySpan<byte> bytes)
    {
        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (bytes.Length >= 8 &&
            bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
            bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            return "image/png";

        // JPEG: FF D8 FF
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "image/jpeg";

        // WebP: "RIFF" .... "WEBP"
        if (bytes.Length >= 12 &&
            bytes[0] == (byte)'R' && bytes[1] == (byte)'I' && bytes[2] == (byte)'F' && bytes[3] == (byte)'F' &&
            bytes[8] == (byte)'W' && bytes[9] == (byte)'E' && bytes[10] == (byte)'B' && bytes[11] == (byte)'P')
            return "image/webp";

        return null;
    }
}
