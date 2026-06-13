using ControleDisciplinas.Application.Interfaces;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Options;

namespace ControleDisciplinas.Infrastructure.Security;

/// <summary>
/// Hash de senha com Argon2id (decisão D8). MD5 e SHA-256 simples são proibidos
/// para senhas pelo arquivo normativo do projeto.
/// </summary>
public sealed class Argon2PasswordHasher(IOptions<PasswordHashOptions> options) : IPasswordHasher
{
    private readonly PasswordHashOptions _opt = options.Value;

    public string Hash(string senha)
    {
        return Argon2.Hash(
            password: senha,
            secret: Pepper(),
            timeCost: _opt.TimeCost,
            memoryCost: _opt.MemoryCostKib,
            parallelism: _opt.Lanes,
            type: Argon2Type.HybridAddressing, // Argon2id
            hashLength: _opt.HashLength);
    }

    public bool Verificar(string senha, string hash)
    {
        return Argon2.Verify(hash, senha, Pepper());
    }

    private string? Pepper() => string.IsNullOrEmpty(_opt.Pepper) ? null : _opt.Pepper;
}
