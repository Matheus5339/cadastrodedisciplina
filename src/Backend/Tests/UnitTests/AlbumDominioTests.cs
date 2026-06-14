using AlbumFigurinhas.Domain.Entities;
using AlbumFigurinhas.Domain.Exceptions;
using Xunit;

namespace AlbumFigurinhas.Tests.UnitTests;

public class AlbumDominioTests
{
    [Fact]
    public void Album_Atualizar_ValidaPaginas()
    {
        var album = new Album("FIFA", 12);
        Assert.Equal(12, album.Paginas);
        Assert.Throws<ValidacaoException>(() => album.Atualizar("FIFA", 0));
    }

    [Fact]
    public void Figurinha_DefinirImagem_GuardaTag()
    {
        var f = new Figurinha(albumId: 1, numero: 1, nome: "Neymar", pagina: 1, descricao: null);
        f.DefinirImagem([1, 2, 3], "image/png", "abcdef", tamanhoMaximoBytes: 1024);
        Assert.Equal("abcdef", f.Tag);
        Assert.NotNull(f.Imagem);
    }

    [Fact]
    public void Figurinha_NumeroInvalido_Lanca()
    {
        Assert.Throws<ValidacaoException>(() => new Figurinha(1, 0, "X", 1, null));
    }
}
