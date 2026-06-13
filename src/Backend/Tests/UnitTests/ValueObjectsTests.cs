using ControleDisciplinas.Application.Validators;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.ValueObjects;
using Xunit;

namespace ControleDisciplinas.Tests.UnitTests;

public class ValueObjectsTests
{
    [Theory]
    [InlineData("aluno@ucp.edu.br")]
    [InlineData("  MAIUSCULO@Dominio.COM  ")]
    public void Email_Valido_NormalizaParaMinusculo(string entrada)
    {
        var email = Email.Criar(entrada);
        Assert.Equal(entrada.Trim().ToLowerInvariant(), email.Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("sem-arroba")]
    [InlineData("a@b")]
    [InlineData("a b@dominio.com")]
    public void Email_Invalido_Lanca(string? entrada)
    {
        Assert.Throws<ValidacaoException>(() => Email.Criar(entrada));
    }

    [Theory]
    [InlineData("529.982.247-25")] // válido com máscara
    [InlineData("52998224725")]
    public void Cpf_Valido_GuardaSoDigitos(string entrada)
    {
        Assert.Equal("52998224725", Cpf.Criar(entrada).Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("11111111111")] // dígitos repetidos
    [InlineData("52998224724")] // DV errado
    public void Cpf_Invalido_Lanca(string? entrada)
    {
        Assert.Throws<ValidacaoException>(() => Cpf.Criar(entrada));
    }

    [Theory]
    [InlineData("curta1")] // < 8
    [InlineData("somenteletras")] // sem número
    [InlineData("12345678")] // sem letra
    public void Senha_ForaDaPolitica_Lanca(string senha)
    {
        Assert.Throws<ValidacaoException>(() => SenhaValidator.Validar(senha));
    }

    [Fact]
    public void Senha_Valida_Passa()
    {
        SenhaValidator.Validar("Demo@123456");
    }
}
