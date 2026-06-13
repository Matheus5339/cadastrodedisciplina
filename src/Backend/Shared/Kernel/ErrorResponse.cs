namespace ControleDisciplinas.Shared.Kernel;

/// <summary>Formato padronizado de erro devolvido pela API.</summary>
public sealed record ErrorResponse(int Status, string Titulo, string Detalhe, string? TraceId);
