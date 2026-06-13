using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Shared.Constants;

namespace ControleDisciplinas.Application.Validators;

/// <summary>Limite de tamanho e tipos permitidos da foto (regra de segurança 9).</summary>
public static class FotoValidator
{
    public static void Validar(long tamanhoBytes, string? contentType)
    {
        if (tamanhoBytes <= 0)
            throw new ValidacaoException("Arquivo de foto vazio.");
        if (tamanhoBytes > FotoConstants.TamanhoMaximoBytes)
            throw new ValidacaoException("Foto excede o limite de 2 MB.");
        if (string.IsNullOrWhiteSpace(contentType) || !FotoConstants.TiposPermitidos.ContainsKey(contentType))
            throw new ValidacaoException("Tipo de foto não permitido. Use JPEG, PNG ou WebP.");
    }
}
