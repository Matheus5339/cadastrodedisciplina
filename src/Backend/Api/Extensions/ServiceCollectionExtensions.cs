using System.Text;
using ControleDisciplinas.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ControleDisciplinas.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string CorsPolicy = "FrontendRestrito";

    /// <summary>Autenticação JWT Bearer com validação completa (segurança 2).</summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.Secao).Get<JwtOptions>() ?? new JwtOptions();
        if (string.IsNullOrWhiteSpace(jwt.Secret) || jwt.Secret.Length < JwtOptions.TamanhoMinimoSecret)
            throw new InvalidOperationException(
                "Jwt:Secret ausente ou curto demais (mínimo 32 caracteres). " +
                "Configure via variável de ambiente Jwt__Secret, user secrets ou appsettings.Development.json (não versionado). " +
                "Veja appsettings.Development.example.json.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                };
            });

        services.AddAuthorization();
        return services;
    }

    /// <summary>CORS restritivo: somente origens configuradas em AllowedOrigins (segurança 5).</summary>
    public static IServiceCollection AddRestrictiveCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origens = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options => options.AddPolicy(CorsPolicy, policy =>
            policy.WithOrigins(origens)
                .AllowAnyHeader()
                .AllowAnyMethod()));
        return services;
    }

    /// <summary>Swagger com suporte a Bearer — registrado sempre, exposto só em desenvolvimento.</summary>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Controle de Disciplinas UCP",
                Version = "v1",
                Description = "API de controle de disciplinas cursadas — Engenharia da Computação, UCP.",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o access token JWT.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    },
                    Array.Empty<string>()
                },
            });
        });
        return services;
    }
}
