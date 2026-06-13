using System.Net;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class AuthFlowTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Registro_Login_Refresh_Logout_FluxoCompleto()
    {
        var (client, auth) = await factory.RegistrarAlunoAsync();
        Assert.NotEmpty(auth.AccessToken);
        Assert.NotEmpty(auth.RefreshToken);

        // login com as mesmas credenciais
        var login = await client.PostAsJsonAsync("/api/auth/login",
            new { email = auth.Aluno.Email, senha = "SenhaForte123" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // refresh com rotação: o novo token funciona, o antigo é revogado
        var refresh1 = await client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = auth.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, refresh1.StatusCode);
        var rotacionado = (await refresh1.Content.ReadFromJsonAsync<AuthResultDto>())!;
        Assert.NotEqual(auth.RefreshToken, rotacionado.RefreshToken);

        // reuso do refresh token antigo deve ser rejeitado (rotação/revogação)
        var reuso = await client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = auth.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, reuso.StatusCode);

        // o reuso revoga a família inteira: o token rotacionado também morre
        var aposReuso = await client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = rotacionado.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, aposReuso.StatusCode);

        // novo login e logout invalidam o refresh token (segurança 16)
        var login2 = (await (await client.PostAsJsonAsync("/api/auth/login",
            new { email = auth.Aluno.Email, senha = "SenhaForte123" })).Content.ReadFromJsonAsync<AuthResultDto>())!;
        var logout = await client.PostAsJsonAsync("/api/auth/logout", new { refreshToken = login2.RefreshToken });
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var refreshAposLogout = await client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = login2.RefreshToken });
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
