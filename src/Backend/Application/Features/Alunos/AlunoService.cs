using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Application.Mappings;
using ControleDisciplinas.Application.Validators;
using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Domain.Interfaces;
using ControleDisciplinas.Shared.Constants;

namespace ControleDisciplinas.Application.Features.Alunos;

public interface IAlunoService
{
    Task<AlunoDto> ObterMeusDadosAsync(CancellationToken ct = default);
    Task<AlunoDto> AtualizarMeusDadosAsync(string nome, string email, CancellationToken ct = default);
    Task DefinirFotoAsync(byte[] conteudo, string contentType, CancellationToken ct = default);
    Task<FotoDto> ObterFotoAsync(CancellationToken ct = default);
}

public sealed class AlunoService(
    IAlunoRepository alunos,
    ICurrentUserService usuarioAtual,
    IUnitOfWork uow) : IAlunoService
{
    public async Task<AlunoDto> ObterMeusDadosAsync(CancellationToken ct = default)
    {
        var aluno = await ObterAlunoAsync(ct);
        return aluno.ToDto();
    }

    public async Task<AlunoDto> AtualizarMeusDadosAsync(string nome, string email, CancellationToken ct = default)
    {
        var aluno = await ObterAlunoAsync(ct);

        var emailNormalizado = Domain.ValueObjects.Email.Criar(email).Valor;
        if (await alunos.ExisteEmailAsync(emailNormalizado, ignorarId: aluno.Id, ct: ct))
            throw new ConflitoException("Já existe um aluno cadastrado com este e-mail.");

        aluno.AtualizarPerfil(nome, emailNormalizado);
        await uow.SaveChangesAsync(ct);
        return aluno.ToDto();
    }

    public async Task DefinirFotoAsync(byte[] conteudo, string contentType, CancellationToken ct = default)
    {
        FotoValidator.Validar(conteudo.LongLength, contentType);
        var aluno = await ObterAlunoAsync(ct);
        aluno.DefinirFoto(conteudo, contentType, FotoConstants.TamanhoMaximoBytes);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<FotoDto> ObterFotoAsync(CancellationToken ct = default)
    {
        var aluno = await ObterAlunoAsync(ct);
        if (aluno.Foto is not { Length: > 0 } || aluno.FotoContentType is null)
            throw new NaoEncontradoException("Aluno não possui foto cadastrada.");
        return new FotoDto(aluno.Foto, aluno.FotoContentType);
    }

    private async Task<Domain.Entities.Aluno> ObterAlunoAsync(CancellationToken ct) =>
        await alunos.ObterPorIdAsync(usuarioAtual.AlunoId, ct)
            ?? throw new NaoEncontradoException("Aluno autenticado não encontrado.");
}
