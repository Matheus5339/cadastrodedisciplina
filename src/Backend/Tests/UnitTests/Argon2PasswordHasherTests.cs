using ControleDisciplinas.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace ControleDisciplinas.Tests.UnitTests;

public class Argon2PasswordHasherTests
{
    private static Argon2PasswordHasher CriarHasher(string? pepper = null) =>
        new(Options.Create(new PasswordHashOptions
        {
            // parâmetros reduzidos apenas para velocidade dos testes
            MemoryCostKib = 8192,
            TimeCost = 1,
            Lanes = 1,
            Pepper = pepper,
        }));

    [Fact]
    public void Hash_GeraFormatoArgon2id_EVerificaCorretamente()
    {
        var hasher = CriarHasher();
        var hash = hasher.Hash("SenhaForte123");

        Assert.StartsWith("$argon2id$", hash);
        Assert.True(hasher.Verificar("SenhaForte123", hash));
        Assert.False(hasher.Verificar("senhaErrada1", hash));
    }

    [Fact]
    public void Hash_MesmaSenha_GeraHashesDiferentes_PorCausaDoSalt()
    {
        var hasher = CriarHasher();
        Assert.NotEqual(hasher.Hash("SenhaForte123"), hasher.Hash("SenhaForte123"));
    }

    [Fact]
    public void Pepper_Diferente_InvalidaVerificacao()
    {
        var comPepper = CriarHasher("pepper-secreto");
        var hash = comPepper.Hash("SenhaForte123");

        Assert.True(comPepper.Verificar("SenhaForte123", hash));
        Assert.False(CriarHasher("outro-pepper").Verificar("SenhaForte123", hash));
    }
}
