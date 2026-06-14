using ControleDisciplinas.Domain.Exceptions;
using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

/// <summary>
/// Figurinha do álbum (PDF §9 — FrmFigurinha). A <see cref="Tag"/> é o hash MD5 da
/// imagem, preenchido automaticamente; é a chave de aquisição pelo colecionador.
/// </summary>
public class Figurinha : EntityBase
{
    public int AlbumId { get; private set; }
    public int Numero { get; private set; }
    public string Nome { get; private set; } = null!;
    public int Pagina { get; private set; }
    public string? Descricao { get; private set; }
    public byte[]? Imagem { get; private set; }
    public string? ImagemContentType { get; private set; }
    public string Tag { get; private set; } = null!;

    public Album? Album { get; private set; }

    private Figurinha() { } // EF Core

    public Figurinha(int albumId, int numero, string nome, int pagina, string? descricao)
    {
        AlbumId = albumId;
        Atualizar(numero, nome, pagina, descricao);
        Tag = string.Empty; // definido junto com a imagem
    }

    public void Atualizar(int numero, string nome, int pagina, string? descricao)
    {
        if (numero is < 1 or > 100000)
            throw new ValidacaoException("Número da figurinha deve estar entre 1 e 100000.");
        var n = nome?.Trim();
        if (string.IsNullOrEmpty(n) || n.Length > 150)
            throw new ValidacaoException("Nome da figurinha é obrigatório (até 150 caracteres).");
        if (pagina < 1)
            throw new ValidacaoException("Página deve ser maior ou igual a 1.");
        if (descricao is { Length: > 1000 })
            throw new ValidacaoException("Descrição deve ter até 1000 caracteres.");

        Numero = numero;
        Nome = n;
        Pagina = pagina;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    /// <summary>Define a imagem e a tag (hash MD5) calculada a partir dela.</summary>
    public void DefinirImagem(byte[] conteudo, string contentType, string tagMd5, int tamanhoMaximoBytes)
    {
        if (conteudo is null || conteudo.Length == 0)
            throw new ValidacaoException("Imagem da figurinha vazia.");
        if (conteudo.Length > tamanhoMaximoBytes)
            throw new ValidacaoException($"Imagem excede o limite de {tamanhoMaximoBytes / (1024 * 1024)} MB.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ValidacaoException("Tipo da imagem não informado.");
        if (string.IsNullOrWhiteSpace(tagMd5))
            throw new ValidacaoException("Tag (hash) da imagem não informada.");
        Imagem = conteudo;
        ImagemContentType = contentType;
        Tag = tagMd5;
    }
}
