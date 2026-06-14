using System.Net;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class AuthFlowTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Login_PorNome_RetornaTokenEPerfil()
    {
        var client = factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/api/auth/login", new { nome = "autor", senha = ApiFactory.Senha });
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var auth = (await resp.Content.ReadFromJsonAsync<AuthResponseDto>(ApiFactory.Json))!;
        Assert.NotEmpty(auth.AccessToken);
        Assert.Equal("autor", auth.Usuario.Nome);
        Assert.Equal(Domain.Enums.Perfil.Autor, auth.Usuario.Perfil);
    }

    [Fact]
    public async Task Login_SenhaErrada_Retorna401()
    {
        var client = factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/api/auth/login", new { nome = "admin", senha = "errada123" });
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Refresh_E_Logout_ViaCookie()
    {
        var client = factory.CreateClient(); // HandleCookies = true
        var login = await client.PostAsJsonAsync("/api/auth/login", new { nome = "colecionador", senha = ApiFactory.Senha });
        login.EnsureSuccessStatusCode();

        // refresh usa o cookie httpOnly (sem corpo)
        var refresh = await client.PostAsync("/api/auth/refresh", null);
        Assert.Equal(HttpStatusCode.OK, refresh.StatusCode);

        // logout precisa do bearer
        var auth = (await login.Content.ReadFromJsonAsync<AuthResponseDto>(ApiFactory.Json))!;
        var logoutReq = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        logoutReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);
        var logout = await client.SendAsync(logoutReq);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        // após logout o cookie é limpo: refresh falha
        var aposLogout = await client.PostAsync("/api/auth/refresh", null);
        Assert.Equal(HttpStatusCode.Unauthorized, aposLogout.StatusCode);
    }
}
