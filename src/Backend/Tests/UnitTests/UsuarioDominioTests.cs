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
        var u = new Usuario("Maria", "hash", Perfil.Autor);
        Assert.Equal("Maria", u.Nome);
        Assert.Equal(Perfil.Autor, u.Perfil);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Criar_ComNomeInvalido_Lanca(string nome)
    {
        Assert.Throws<ValidacaoException>(() => new Usuario(nome, "hash", Perfil.Colecionador));
    }

    [Fact]
    public void AtualizarProprioNome_NaoAlteraPerfil()
    {
        var u = new Usuario("Joao", "hash", Perfil.Colecionador);
        u.AtualizarProprioNome("Joao Silva");
        Assert.Equal("Joao Silva", u.Nome);
        Assert.Equal(Perfil.Colecionador, u.Perfil); // perfil permanece
    }
}
