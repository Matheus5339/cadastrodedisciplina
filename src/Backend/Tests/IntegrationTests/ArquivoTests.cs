using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class ArquivoTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task ExportarTexto_ContemAsFigurinhas()
    {
        var autor = await factory.ClienteAutorAsync();
        await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(1, "Neymar", 1, null, 1));
        await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(2, "Messi", 2, null, 2));

        var resp = await autor.GetAsync("/api/arquivos/figurinhas/texto");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var texto = await resp.Content.ReadAsStringAsync();
        Assert.Contains("Neymar", texto);
        Assert.Contains("Messi", texto);
    }

    [Fact]
    public async Task ExportarBinario_E_ImportarDeVolta()
    {
        var autor = await factory.ClienteAutorAsync();
        await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(10, "A", 1, null, 10));
        await autor.PostAsync("/api/figurinhas", ApiFactory.FormFigurinha(11, "B", 1, null, 11));

        // exporta o binário
        var bin = await autor.GetByteArrayAsync("/api/arquivos/figurinhas/binario");
        Assert.True(bin.Length > 0);

        // limpa tudo
        await autor.PostAsync("/api/figurinhas/limpar", null);
        Assert.Empty((await autor.GetFromJsonAsync<List<FigurinhaDto>>("/api/figurinhas"))!);

        // importa de volta
        var form = new MultipartFormDataContent();
        var conteudo = new ByteArrayContent(bin);
        conteudo.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(conteudo, "arquivo", "figurinhas.figb");
        var imp = await autor.PostAsync("/api/arquivos/figurinhas/binario", form);
        imp.EnsureSuccessStatusCode();
        var corpo = await imp.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        Assert.Equal(2, corpo!["importadas"]);

        Assert.Equal(2, (await autor.GetFromJsonAsync<List<FigurinhaDto>>("/api/figurinhas"))!.Count);
    }
}
