using AlbumFigurinhas.Shared.Constants;
using Xunit;

namespace AlbumFigurinhas.Tests.UnitTests;

/// <summary>
/// Validação de imagem por ASSINATURA (magic bytes) — endurecimento de upload:
/// o tipo é determinado pelos bytes reais, não pelo Content-Type do cliente.
/// </summary>
public class FotoAssinaturaTests
{
    private static readonly byte[] PngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
    private static readonly byte[] JpegHeader = [0xFF, 0xD8, 0xFF, 0xE0, 0x00];
    private static readonly byte[] WebpHeader =
        [(byte)'R', (byte)'I', (byte)'F', (byte)'F', 0, 0, 0, 0, (byte)'W', (byte)'E', (byte)'B', (byte)'P'];

    [Fact]
    public void Detecta_Png() => Assert.Equal("image/png", FotoConstants.DetectarContentType(PngHeader));

    [Fact]
    public void Detecta_Jpeg() => Assert.Equal("image/jpeg", FotoConstants.DetectarContentType(JpegHeader));

    [Fact]
    public void Detecta_Webp() => Assert.Equal("image/webp", FotoConstants.DetectarContentType(WebpHeader));

    [Fact]
    public void Rejeita_ConteudoNaoImagem()
    {
        // HTML/SVG/texto disfarçado de imagem -> não passa
        Assert.Null(FotoConstants.DetectarContentType("<svg onload=alert(1)>"u8.ToArray()));
        Assert.Null(FotoConstants.DetectarContentType("não é imagem"u8.ToArray()));
        Assert.Null(FotoConstants.DetectarContentType([]));
    }

    [Fact]
    public void Rejeita_Gif_NaoSuportado()
    {
        var gif = "GIF89a"u8.ToArray();
        Assert.Null(FotoConstants.DetectarContentType(gif));
    }
}
