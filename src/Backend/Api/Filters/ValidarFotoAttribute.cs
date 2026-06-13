using ControleDisciplinas.Application.Validators;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ControleDisciplinas.Api.Filters;

/// <summary>
/// Filtro de ação que valida o upload da foto (tamanho e tipo) antes de o
/// controller executar — rejeita cedo sem carregar o serviço (segurança 9).
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidarFotoAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var arquivo = context.HttpContext.Request.Form.Files.FirstOrDefault();
        if (arquivo is null)
            throw new Domain.Exceptions.ValidacaoException("Nenhum arquivo de foto enviado (campo 'foto').");

        FotoValidator.Validar(arquivo.Length, arquivo.ContentType);
        base.OnActionExecuting(context);
    }
}
