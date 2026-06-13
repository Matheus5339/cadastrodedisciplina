using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Domain.Entities;

namespace ControleDisciplinas.Application.Mappings;

public static class Mappers
{
    public static AlunoDto ToDto(this Aluno a) =>
        new(a.Id, a.Rgu, a.Cpf, a.Email, a.Nome, a.Foto is { Length: > 0 });

    public static DisciplinaDto ToDto(this Disciplina d) =>
        new(d.Id, d.Codigo, d.Nome, d.Professor, d.Periodo, d.Creditos);

    public static HistoricoDto ToDto(this Historico h) =>
        new(h.Id,
            h.DisciplinaId,
            h.Disciplina?.Codigo ?? string.Empty,
            h.Disciplina?.Nome ?? string.Empty,
            h.Disciplina?.Professor,
            h.Disciplina?.Creditos ?? 0,
            h.Ano,
            h.Semestre,
            h.Periodo,
            h.MediaFinal);
}
