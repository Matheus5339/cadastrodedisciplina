using System.Net;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class FotoEHealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static MultipartFormDataContent CriarFormFoto(byte[] bytes, string contentType, string nome = "foto.jpg")
    {
        var conteudo = new ByteArrayContent(bytes);
        conteudo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        return new MultipartFormDataContent { { conteudo, "foto", nome } };
    }

    [Fact]
    public async Task UploadEConsultaDeFoto_FluxoCompleto()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();

        // sem foto ainda -> 404
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/alunos/me/foto")).StatusCode);

        var bytes = new byte[1024];
        Random.Shared.NextBytes(bytes);
        var upload = await client.PutAsync("/api/alunos/me/foto", CriarFormFoto(bytes, "image/jpeg"));
        Assert.Equal(HttpStatusCode.NoContent, upload.StatusCode);

        var consulta = await client.GetAsync("/api/alunos/me/foto");
        Assert.Equal(HttpStatusCode.OK, consulta.StatusCode);
        Assert.Equal("image/jpeg", consulta.Content.Headers.ContentType!.MediaType);
        Assert.Equal(bytes, await consulta.Content.ReadAsByteArrayAsync());

        var me = await client.GetFromJsonAsync<AlunoDto>("/api/alunos/me");
        Assert.True(me!.PossuiFoto);
    }

    [Fact]
    public async Task Foto_AcimaDe2MB_Rejeitada()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var grande = new byte[2 * 1024 * 1024 + 1];
        var resposta = await client.PutAsync("/api/alunos/me/foto", CriarFormFoto(grande, "image/png", "grande.png"));
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Foto_TipoNaoPermitido_Rejeitada()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var resposta = await client.PutAsync("/api/alunos/me/foto", CriarFormFoto([1, 2, 3], "application/pdf", "doc.pdf"));
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task AtualizacaoDePerfil_AlteraNomeEEmail()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var novoEmail = $"novo-{Guid.NewGuid().ToString("N")[..8]}@teste.ucp.edu.br";

        var resposta = await client.PutAsJsonAsync("/api/alunos/me", new { nome = "Nome Atualizado", email = novoEmail });
        resposta.EnsureSuccessStatusCode();

        var me = await client.GetFromJsonAsync<AlunoDto>("/api/alunos/me");
        Assert.Equal("Nome Atualizado", me!.Nome);
        Assert.Equal(novoEmail, me.Email);
    }

    [Fact]
    public async Task HealthCheck_RespondeSemExporDetalhes()
    {
        var client = factory.CreateClient();
        var resposta = await client.GetAsync("/health");
        var corpo = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("Healthy", corpo); // só o status, nada de connection string ou versões
    }

    [Fact]
    public async Task Swagger_NaoExposto_ForaDeDesenvolvimento()
    {
        // ambiente de teste é "Testing", não "Development" -> Swagger desligado
        var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/swagger/index.html")).StatusCode);
    }
}
