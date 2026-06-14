using System.Net;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class UsuariosEAutorizacaoTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Admin_CRUD_Filtro_E_ResetSenha()
    {
        var admin = await factory.ClienteAdminAsync();

        // lista inicial (admin/autor/colecionador)
        var iniciais = (await admin.GetFromJsonAsync<List<UsuarioDto>>("/api/usuarios", ApiFactory.Json))!;
        Assert.True(iniciais.Count >= 3);

        // criar
        var criar = await admin.PostAsJsonAsync("/api/usuarios",
            new { login = "Pedro", senha = "Pedro@123", perfil = "Colecionador" });
        Assert.Equal(HttpStatusCode.Created, criar.StatusCode);
        var pedro = (await criar.Content.ReadFromJsonAsync<UsuarioDto>(ApiFactory.Json))!;

        // filtrar por "Ped"
        var filtrados = (await admin.GetFromJsonAsync<List<UsuarioDto>>("/api/usuarios?filtro=Ped", ApiFactory.Json))!;
        Assert.Single(filtrados);
        Assert.Equal("Pedro", filtrados[0].Login);

        // atualizar
        var atualizar = await admin.PutAsJsonAsync($"/api/usuarios/{pedro.Id}", new { login = "PedroSouza", perfil = "Autor" });
        Assert.Equal(HttpStatusCode.OK, atualizar.StatusCode);

        // resetar senha → senha padrão
        var reset = await admin.PostAsync($"/api/usuarios/{pedro.Id}/resetar-senha", null);
        reset.EnsureSuccessStatusCode();
        var corpo = await reset.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.Equal("123456", corpo!["senha"]);

        // o novo login com a senha padrão funciona
        var loginPadrao = await factory.CreateClient().PostAsJsonAsync("/api/auth/login", new { login = "PedroSouza", senha = "123456" });
        Assert.Equal(HttpStatusCode.OK, loginPadrao.StatusCode);

        // remover
        var remover = await admin.DeleteAsync($"/api/usuarios/{pedro.Id}");
        Assert.Equal(HttpStatusCode.NoContent, remover.StatusCode);
    }

    [Fact]
    public async Task NaoAdmin_AcessoAUsuarios_Retorna403()
    {
        var autor = await factory.ClienteAutorAsync();
        var resp = await autor.GetAsync("/api/usuarios");
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task SemToken_Retorna401()
    {
        var resp = await factory.CreateClient().GetAsync("/api/usuarios");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
