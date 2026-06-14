using System.Net;
using System.Net.Http.Json;
using AlbumFigurinhas.Application.DTOs;
using AlbumFigurinhas.Tests.TestUtilities;
using Xunit;

namespace AlbumFigurinhas.Tests.IntegrationTests;

public class ImagemEHealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Health_Anonimo_Retorna200()
    {
        var resp = await factory.CreateClient().GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Figurinha_ImagemNoBanco_PodeSerBaixada()
    {
        var autor = await factory.ClienteAutorAsync();
        var criar = await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(1, "Messi", 1, null, 11));
        var fig = (await criar.Content.ReadFromJsonAsync<FigurinhaDto>())!;

        var img = await autor.GetAsync($"/api/figurinhas/{fig.Id}/imagem");
        Assert.Equal(HttpStatusCode.OK, img.StatusCode);
        Assert.Equal("image/png", img.Content.Headers.ContentType?.MediaType);
        Assert.True((await img.Content.ReadAsByteArrayAsync()).Length > 0);
    }
}
