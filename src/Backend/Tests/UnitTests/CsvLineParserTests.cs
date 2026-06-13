using ControleDisciplinas.Shared.Helpers;
using Xunit;

namespace ControleDisciplinas.Tests.UnitTests;

public class CsvLineParserTests
{
    [Fact]
    public void LinhaSimples_SeparadaPorVirgula()
    {
        var campos = CsvLineParser.Parse("ECOMP-001,Calculo I,Fulano,1,4");
        Assert.Equal(["ECOMP-001", "Calculo I", "Fulano", "1", "4"], campos);
    }

    [Fact]
    public void CampoEntreAspas_PreservaVirgulaInterna()
    {
        var campos = CsvLineParser.Parse("ECOMP-022,\"Fluidos, Ondas e Calor\",Prof,3,4");
        Assert.Equal(5, campos.Count);
        Assert.Equal("Fluidos, Ondas e Calor", campos[1]);
    }

    [Fact]
    public void AspasEscapadas_ViramAspasSimples()
    {
        var campos = CsvLineParser.Parse("a,\"diz \"\"oi\"\"\",b");
        Assert.Equal("diz \"oi\"", campos[1]);
    }

    [Fact]
    public void CampoVazio_EhPreservado()
    {
        var campos = CsvLineParser.Parse("ECOMP-001,Nome,,1,4");
        Assert.Equal(string.Empty, campos[2]);
    }
}
