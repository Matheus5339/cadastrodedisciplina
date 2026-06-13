namespace ControleDisciplinas.Application.DTOs;

public sealed record AuthResultDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    AlunoDto Aluno);

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
