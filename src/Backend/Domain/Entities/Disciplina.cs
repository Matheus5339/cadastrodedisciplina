using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

public class Disciplina : EntityBase
{
    public string Codigo { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public string? Professor { get; private set; }
    public int Periodo { get; private set; }
    public int Creditos { get; private set; }

    private Disciplina() { } // EF Core

    public Disciplina(string codigo, string nome, string? professor, int periodo, int creditos)
    {
        Atualizar(codigo, nome, professor, periodo, creditos);
    }

    public void Atualizar(string codigo, string nome, string? professor, int periodo, int creditos)
    {
        var cod = codigo?.Trim();
        if (string.IsNullOrEmpty(cod) || cod.Length > 20)
            throw new ValidacaoException("Código da disciplina é obrigatório (até 20 caracteres).");

        var nom = nome?.Trim();
        if (string.IsNullOrEmpty(nom) || nom.Length > 150)
            throw new ValidacaoException("Nome da disciplina é obrigatório (até 150 caracteres).");

        if (periodo is < 1 or > 20)
            throw new ValidacaoException("Período deve estar entre 1 e 20.");
        if (creditos is < 0 or > 30)
            throw new ValidacaoException("Créditos devem estar entre 0 e 30.");

        Codigo = cod;
        Nome = nom;
        Professor = string.IsNullOrWhiteSpace(professor) ? null : professor.Trim();
        Periodo = periodo;
        Creditos = creditos;
    }
}
