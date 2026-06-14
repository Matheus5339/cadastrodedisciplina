using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Enums;
using ControleDisciplinas.Domain.Exceptions;
using Xunit;

namespace ControleDisciplinas.Tests.UnitTests;

public class UsuarioDominioTests
{
    [Fact]
    public void Criar_ComDadosValidos_DefineCampos()
    {
        var u = new Usuario("maria", "hash", Perfil.Autor);
        Assert.Equal("maria", u.Login);
        Assert.Equal(Perfil.Autor, u.Perfil);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Criar_ComLoginInvalido_Lanca(string login)
    {
        Assert.Throws<ValidacaoException>(() => new Usuario(login, "hash", Perfil.Colecionador));
    }

    [Fact]
    public void AtualizarProprioLogin_NaoAlteraPerfil()
    {
        var u = new Usuario("joao", "hash", Perfil.Colecionador);
        u.AtualizarProprioLogin("joao.silva");
        Assert.Equal("joao.silva", u.Login);
        Assert.Equal(Perfil.Colecionador, u.Perfil); // perfil permanece
    }
}
