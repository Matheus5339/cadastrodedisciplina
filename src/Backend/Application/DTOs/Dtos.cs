namespace ControleDisciplinas.Application.DTOs;

/// <summary>
/// Resultado interno da autenticação. Inclui o refresh token bruto, que o
/// controller usa para emitir o cookie httpOnly — nunca vai no corpo da resposta.
/// </summary>
public sealed record AuthResultDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    AlunoDto Aluno);

/// <summary>Corpo retornado ao cliente: sem o refresh token (que viaja em cookie httpOnly).</summary>
public sealed record AuthResponseDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    AlunoDto Aluno)
{
    public static AuthResponseDto De(AuthResultDto r) => new(r.AccessToken, r.AccessTokenExpiresAtUtc, r.Aluno);
}

public sealed record AlunoDto(
    int Id,
    string Rgu,
    string Cpf,
    string Email,
    string Nome,
    bool PossuiFoto);

public sealed record FotoDto(byte[] Conteudo, string ContentType);

public sealed record DisciplinaDto(
    int Id,
    string Codigo,
    string Nome,
    string? Professor,
    int Periodo,
    int Creditos);

public sealed record HistoricoDto(
    int Id,
    int DisciplinaId,
    string DisciplinaCodigo,
    string DisciplinaNome,
    string? DisciplinaProfessor,
    int Creditos,
    int Ano,
    int Semestre,
    int Periodo,
    decimal MediaFinal);

public sealed record CrDto(decimal? Cr, int TotalCreditos, int TotalDisciplinas);
