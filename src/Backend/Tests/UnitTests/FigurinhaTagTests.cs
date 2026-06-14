using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace ControleDisciplinas.Tests.UnitTests;

/// <summary>Verifica o cálculo da tag (hash MD5 da imagem) — item da rubrica.</summary>
public class FigurinhaTagTests
{
    private static string Tag(byte[] bytes) => Convert.ToHexString(MD5.HashData(bytes)).ToLowerInvariant();

    [Fact]
    public void Tag_DeConteudoConhecido_CorrespondeAoHashEsperado()
    {
        // hash de "abc" = 900150983cd24fb0d6963f7d28e17f72
        Assert.Equal("900150983cd24fb0d6963f7d28e17f72", Tag(Encoding.ASCII.GetBytes("abc")));
    }

    [Fact]
    public void Tag_TemTrintaEDoisHex_EMudaComOConteudo()
    {
        var a = Tag([1, 2, 3]);
        var b = Tag([1, 2, 4]);
        Assert.Equal(32, a.Length);
        Assert.NotEqual(a, b);
    }
}
