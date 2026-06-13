using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ControleDisciplinas.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;
    private readonly SigningCredentials _credentials;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _opt = options.Value;
        if (string.IsNullOrWhiteSpace(_opt.Secret) || _opt.Secret.Length < JwtOptions.TamanhoMinimoSecret)
            throw new InvalidOperationException(
                $"Jwt:Secret ausente ou menor que {JwtOptions.TamanhoMinimoSecret} caracteres. " +
                "Configure via variável de ambiente Jwt__Secret, user secrets ou appsettings.Development.json (não versionado).");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret));
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public int RefreshTokenDias => _opt.RefreshTokenDias;

    public (string Token, DateTime ExpiresAtUtc) GerarAccessToken(Aluno aluno)
    {
        var agora = DateTime.UtcNow;
        var expira = agora.AddMinutes(_opt.AccessTokenMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, aluno.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, aluno.Email),
            new(JwtRegisteredClaimNames.Name, aluno.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: agora,
            expires: expira,
            signingCredentials: _credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }

    public string GerarRefreshToken()
    {
        // 64 bytes aleatórios — token opaco, nunca armazenado em claro
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashRefreshToken(string refreshToken)
    {
        // SHA-256 aqui é hash de TOKEN opaco de alta entropia, não de senha (decisão D8)
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
