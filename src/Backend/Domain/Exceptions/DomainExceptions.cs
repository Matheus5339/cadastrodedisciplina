namespace ControleDisciplinas.Domain.Exceptions;

/// <summary>Base das exceções de domínio/aplicação mapeadas pelo middleware da API.</summary>
public abstract class DomainException(string mensagem) : Exception(mensagem);

/// <summary>Regra de negócio ou entrada inválida (HTTP 400).</summary>
public sealed class ValidacaoException(string mensagem) : DomainException(mensagem);

/// <summary>Recurso inexistente ou fora do escopo do aluno autenticado (HTTP 404).</summary>
public sealed class NaoEncontradoException(string mensagem) : DomainException(mensagem);

/// <summary>Conflito com estado existente, ex.: unicidade (HTTP 409).</summary>
public sealed class ConflitoException(string mensagem) : DomainException(mensagem);

/// <summary>Credenciais ou token inválidos (HTTP 401).</summary>
public sealed class NaoAutorizadoException(string mensagem) : DomainException(mensagem);
