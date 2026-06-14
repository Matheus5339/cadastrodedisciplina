using System.Text.Json;
using AlbumFigurinhas.Domain.Exceptions;
using AlbumFigurinhas.Shared.Kernel;

namespace AlbumFigurinhas.Api.Middlewares;

/// <summary>Tratamento global de exceções com resposta padronizada (segurança 8).</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, titulo) = ex switch
            {
                ValidacaoException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                NaoAutorizadoException => (StatusCodes.Status401Unauthorized, "Não autorizado"),
                NaoEncontradoException => (StatusCodes.Status404NotFound, "Não encontrado"),
                ConflitoException => (StatusCodes.Status409Conflict, "Conflito"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno"),
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Erro não tratado em {Method} {Path}", context.Request.Method, context.Request.Path);
            else
                logger.LogInformation("Falha de negócio ({Status}) em {Method} {Path}: {Mensagem}",
                    status, context.Request.Method, context.Request.Path, ex.Message);

            // nunca vazar stack trace ou detalhes internos em erros 500
            var detalhe = status == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : ex.Message;

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json; charset=utf-8";
            var corpo = new ErrorResponse(status, titulo, detalhe, context.TraceIdentifier);
            await context.Response.WriteAsync(JsonSerializer.Serialize(corpo, JsonOptions));
        }
    }
}
