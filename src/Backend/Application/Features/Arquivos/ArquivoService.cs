using System.Text;
using AlbumFigurinhas.Application.Features.Figurinhas;
using AlbumFigurinhas.Domain.Exceptions;
using AlbumFigurinhas.Domain.Interfaces;

namespace AlbumFigurinhas.Application.Features.Arquivos;

/// <summary>
/// Importação/exportação de figurinhas em arquivo TEXTO e BINÁRIO
/// (itens da rubrica: "arq texto" e "arq binario").
/// </summary>
public interface IArquivoService
{
    Task<string> ExportarTextoAsync(CancellationToken ct = default);
    Task<byte[]> ExportarBinarioAsync(CancellationToken ct = default);
    Task<int> ImportarBinarioAsync(byte[] conteudo, CancellationToken ct = default);
}

public sealed class ArquivoService(
    IFigurinhaRepository figurinhas,
    IAlbumRepository albuns,
    IFigurinhaService figurinhaService) : IArquivoService
{
    private const int Magic = 0x46494742; // "FIGB"
    private const int Versao = 1;

    public async Task<string> ExportarTextoAsync(CancellationToken ct = default)
    {
        var album = await ObterAlbumAsync(ct);
        var lista = await figurinhas.ListarAsync(album.Id, new FigurinhaFiltro(null, null), ct);

        var sb = new StringBuilder();
        sb.AppendLine($"# Álbum: {album.Nome} ({album.Paginas} páginas)");
        sb.AppendLine($"# Figurinhas: {lista.Count}");
        sb.AppendLine("# numero;nome;pagina;tag;descricao");
        foreach (var f in lista)
            sb.AppendLine($"{f.Numero};{f.Nome};{f.Pagina};{f.Tag};{(f.Descricao ?? string.Empty).Replace('\n', ' ')}");
        return sb.ToString();
    }

    public async Task<byte[]> ExportarBinarioAsync(CancellationToken ct = default)
    {
        var album = await ObterAlbumAsync(ct);
        var lista = await figurinhas.ListarAsync(album.Id, new FigurinhaFiltro(null, null), ct);

        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            w.Write(Magic);
            w.Write(Versao);
            w.Write(album.Nome);
            w.Write(album.Paginas);
            w.Write(lista.Count);
            foreach (var f in lista)
            {
                w.Write(f.Numero);
                w.Write(f.Nome);
                w.Write(f.Pagina);
                w.Write(f.Descricao ?? string.Empty);
                w.Write(f.ImagemContentType ?? string.Empty);
                var img = f.Imagem ?? [];
                w.Write(img.Length);
                w.Write(img);
            }
        }
        return ms.ToArray();
    }

    public async Task<int> ImportarBinarioAsync(byte[] conteudo, CancellationToken ct = default)
    {
        if (conteudo is null || conteudo.Length < 8)
            throw new ValidacaoException("Arquivo binário inválido.");

        using var ms = new MemoryStream(conteudo);
        using var r = new BinaryReader(ms, Encoding.UTF8);

        if (r.ReadInt32() != Magic)
            throw new ValidacaoException("Formato de arquivo não reconhecido.");
        _ = r.ReadInt32();      // versão
        _ = r.ReadString();     // nome do álbum (ignorado: importa para o álbum atual)
        _ = r.ReadInt32();      // páginas
        var total = r.ReadInt32();

        var importadas = 0;
        for (var i = 0; i < total; i++)
        {
            var numero = r.ReadInt32();
            var nome = r.ReadString();
            var pagina = r.ReadInt32();
            var descricao = r.ReadString();
            var contentType = r.ReadString();
            var tamanho = r.ReadInt32();
            var imagem = r.ReadBytes(tamanho);

            if (imagem.Length == 0) continue;
            try
            {
                await figurinhaService.CriarAsync(numero, nome, pagina,
                    string.IsNullOrEmpty(descricao) ? null : descricao, imagem, contentType, ct);
                importadas++;
            }
            catch (ConflitoException)
            {
                // figurinha já existe (número/tag) — ignora na importação
            }
        }
        return importadas;
    }

    private async Task<Domain.Entities.Album> ObterAlbumAsync(CancellationToken ct) =>
        await albuns.ObterAsync(ct) ?? throw new NaoEncontradoException("Álbum não encontrado.");
}
