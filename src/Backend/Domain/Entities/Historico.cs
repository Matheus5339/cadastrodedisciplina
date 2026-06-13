using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

public class Historico : EntityBase
{
    public int AlunoId { get; private set; }
    public int DisciplinaId { get; private set; }
    public int Ano { get; private set; }
    public int Semestre { get; private set; }
    public int Periodo { get; private set; }
    public decimal MediaFinal { get; private set; }

    public Aluno? Aluno { get; private set; }
    public Disciplina? Disciplina { get; private set; }

    private Historico() { } // EF Core

    public Historico(int alunoId, int disciplinaId, int ano, int semestre, int periodo, decimal mediaFinal)
    {
        if (alunoId <= 0) throw new ValidacaoException("Aluno inválido.");
        if (disciplinaId <= 0) throw new ValidacaoException("Disciplina inválida.");
        AlunoId = alunoId;
        DisciplinaId = disciplinaId;
        Atualizar(ano, semestre, periodo, mediaFinal);
    }

    public void Atualizar(int ano, int semestre, int periodo, decimal mediaFinal)
    {
        if (ano is < 1980 or > 2100)
            throw new ValidacaoException("Ano deve estar entre 1980 e 2100.");
        if (semestre is not (1 or 2))
            throw new ValidacaoException("Semestre deve ser 1 ou 2.");
        if (periodo is < 1 or > 20)
            throw new ValidacaoException("Período deve estar entre 1 e 20.");
        if (mediaFinal is < 0 or > 10)
            throw new ValidacaoException("Média final deve estar entre 0 e 10.");

        Ano = ano;
        Semestre = semestre;
        Periodo = periodo;
        MediaFinal = mediaFinal;
    }

    public void TrocarDisciplina(int disciplinaId)
    {
        if (disciplinaId <= 0) throw new ValidacaoException("Disciplina inválida.");
        DisciplinaId = disciplinaId;
    }
}
