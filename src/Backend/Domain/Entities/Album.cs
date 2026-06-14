using AlbumFigurinhas.Domain.Exceptions;
using AlbumFigurinhas.Shared.Kernel;

namespace AlbumFigurinhas.Domain.Entities;

/// <summary>Álbum único da aplicação, editado pelo Autor (PDF §8 — FrmAutoria).</summary>
public class Album : EntityBase
{
    public string Nome { get; private set; } = null!;
    public int Paginas { get; private set; }
    public byte[]? Capa { get; private set; }
    public string? CapaContentType { get; private set; }

    private Album() { } // EF Core

    public Album(string nome, int paginas)
    {
        Atualizar(nome, paginas);
    }

    public void Atualizar(string nome, int paginas)
    {
        var n = nome?.Trim();
        if (string.IsNullOrEmpty(n) || n.Length > 150)
            throw new ValidacaoException("Nome do álbum é obrigatório (até 150 caracteres).");
        if (paginas is < 1 or > 1000)
            throw new ValidacaoException("Número de páginas deve estar entre 1 e 1000.");
        Nome = n;
        Paginas = paginas;
    }

    public void DefinirCapa(byte[] conteudo, string contentType, int tamanhoMaximoBytes)
    {
        if (conteudo is null || conteudo.Length == 0)
            throw new ValidacaoException("Capa vazia.");
        if (conteudo.Length > tamanhoMaximoBytes)
            throw new ValidacaoException($"Capa excede o limite de {tamanhoMaximoBytes / (1024 * 1024)} MB.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ValidacaoException("Tipo da capa não informado.");
        Capa = conteudo;
        CapaContentType = contentType;
    }
}
