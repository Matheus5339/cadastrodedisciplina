using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class AuthFlowTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    // Envia POST /api/auth/refresh anexando manualmente o cookie de refresh (ou nenhum).
    private static Task<HttpResponseMessage> PostRefreshAsync(HttpClient client, string? refreshCookie)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");
        if (refreshCookie is not null)
            req.Headers.Add("Cookie", $"cdu_rt={refreshCookie}");
        return client.SendAsync(req);
    }

    [Fact]
    public async Task Registro_Login_Refresh_Logout_FluxoCompleto()
    {
        var client = factory.CreateRawClient(); // controla os cookies manualmente
        var reg = ApiFactory.NovoRegistro();

        // registro emite o refresh token em cookie httpOnly (e não no corpo)
        var respReg = await client.PostAsJsonAsync("/api/auth/register", reg);
        Assert.Equal(HttpStatusCode.Created, respReg.StatusCode);
        var auth = (await respReg.Content.ReadFromJsonAsync<AuthResponseDto>())!;
        Assert.NotEmpty(auth.AccessToken);
        var rt0 = ApiFactory.RefreshCookie(respReg);
        Assert.False(string.IsNullOrEmpty(rt0));

        // login também emite o cookie
        var login = await client.PostAsJsonAsync("/api/auth/login", new { email = reg.Email, senha = reg.Senha });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // refresh com rotação: o cookie é trocado por um novo
        var refresh1 = await PostRefreshAsync(client, rt0);
        Assert.Equal(HttpStatusCode.OK, refresh1.StatusCode);
        var rt1 = ApiFactory.RefreshCookie(refresh1);
        Assert.False(string.IsNullOrEmpty(rt1));
        Assert.NotEqual(rt0, rt1);

        // reuso do refresh token antigo deve ser rejeitado (rotação/revogação)
        var reuso = await PostRefreshAsync(client, rt0);
        Assert.Equal(HttpStatusCode.Unauthorized, reuso.StatusCode);

        // o reuso revoga a família inteira: o token rotacionado também morre
        var aposReuso = await PostRefreshAsync(client, rt1);
        Assert.Equal(HttpStatusCode.Unauthorized, aposReuso.StatusCode);

        // sem cookie de refresh: 401
        var semCookie = await PostRefreshAsync(client, null);
        Assert.Equal(HttpStatusCode.Unauthorized, semCookie.StatusCode);

        // novo login e logout invalidam o refresh token (segurança 16)
        var login2 = await client.PostAsJsonAsync("/api/auth/login", new { email = reg.Email, senha = reg.Senha });
        var auth2 = (await login2.Content.ReadFromJsonAsync<AuthResponseDto>())!;
        var rt2 = ApiFactory.RefreshCookie(login2);

        var logoutReq = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        logoutReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth2.AccessToken);
        logoutReq.Headers.Add("Cookie", $"cdu_rt={rt2}");
        var logout = await client.SendAsync(logoutReq);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var refreshAposLogout = await PostRefreshAsync(client, rt2);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshAposLogout.StatusCode);
    }

    [Fact]
    public async Task SenhaErrada_EEmailInexistente_Retornam401SemDistincao()
    {
        var (client, auth) = await factory.RegistrarAlunoAsync();

        var senhaErrada = await client.PostAsJsonAsync("/api/auth/login", new { email = auth.Aluno.Email, senha = "Errada12345" });
        var emailInexistente = await client.PostAsJsonAsync("/api/auth/login", new { email = "nao-existe@x.com", senha = "Qualquer123" });

        Assert.Equal(HttpStatusCode.Unauthorized, senhaErrada.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, emailInexistente.StatusCode);
    }

    [Fact]
    public async Task Registro_EmailCpfRguDuplicados_Retorna409()
    {
        var (client, auth) = await factory.RegistrarAlunoAsync();

        var duplicado = await client.PostAsJsonAsync("/api/auth/register", new
        {
            rgu = "999999",
            cpf = ApiFactory.GerarCpfValido(987654),
            email = auth.Aluno.Email, // e-mail repetido
            nome = "Outro Aluno",
            senha = "SenhaForte123",
        });

        Assert.Equal(HttpStatusCode.Conflict, duplicado.StatusCode);
    }

    [Fact]
    public async Task Registro_EmailInvalido_Retorna400()
    {
        var client = factory.CreateClient();
        var resposta = await client.PostAsJsonAsync("/api/auth/register", new
        {
            rgu = "888888",
            cpf = ApiFactory.GerarCpfValido(111222),
            email = "email-invalido",
            nome = "Aluno",
            senha = "SenhaForte123",
        });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task EndpointProtegido_SemToken_Retorna401()
    {
        var client = factory.CreateClient();
        var resposta = await client.GetAsync("/api/disciplinas");
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }
}
