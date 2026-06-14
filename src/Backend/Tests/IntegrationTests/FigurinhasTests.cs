using System.Net;
using System.Net.Http.Json;
using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Tests.TestUtilities;
using Xunit;

namespace AlbumFigurinhas.Tests.IntegrationTests;

public class FigurinhasTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Autor_CriaFigurinha_ComTagMd5_E_Lista()
    {
        var autor = await factory.ClienteAutorAsync();

        var resp = await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(1, "Neymar", 1, "craque", 101));
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        var fig = (await resp.Content.ReadFromJsonAsync<FigurinhaDto>())!;
        Assert.Equal(32, fig.Tag.Length); // tag = md5 hex
        Assert.True(fig.PossuiImagem);

        var lista = (await autor.GetFromJsonAsync<List<FigurinhaDto>>("/api/figurinhas"))!;
        Assert.Contains(lista, f => f.Nome == "Neymar");

        // filtro por texto
        var filtrada = (await autor.GetFromJsonAsync<List<FigurinhaDto>>("/api/figurinhas?texto=Neym"))!;
        Assert.Single(filtrada);
    }

    [Fact]
    public async Task NumeroDuplicado_Retorna409()
    {
        var autor = await factory.ClienteAutorAsync();
        await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(50, "A", 1, null, 201));
        var dup = await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(50, "B", 1, null, 202));
        Assert.Equal(HttpStatusCode.Conflict, dup.StatusCode);
    }

    [Fact]
    public async Task Colecionador_NaoCriaFigurinha_403()
    {
        var col = await factory.ClienteColecionadorAsync();
        var resp = await col.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(9, "X", 1, null, 301));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task Autor_LimparRemoveTodas()
    {
        var autor = await factory.ClienteAutorAsync();
        await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(70, "C", 1, null, 401));
        var limpar = await autor.PostAsync("/api/figurinhas/limpar", null);
        Assert.Equal(HttpStatusCode.NoContent, limpar.StatusCode);
        var lista = (await autor.GetFromJsonAsync<List<FigurinhaDto>>("/api/figurinhas"))!;
        Assert.Empty(lista);
    }
}
