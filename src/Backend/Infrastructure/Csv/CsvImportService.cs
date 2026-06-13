using System.Security.Cryptography;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Interfaces;
using ControleDisciplinas.Shared.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ControleDisciplinas.Infrastructure.Csv;

/// <summary>
/// Importa o catálogo de disciplinas de um CSV (cabeçalho:
/// codigo,nome,professor,periodo,creditos). Executado SOMENTE na inicialização
/// da aplicação (requisito do normativo §2). Linhas inválidas são contadas e
/// ignoradas; códigos já existentes não são duplicados.
/// </summary>
public sealed class CsvImportService(
    IDisciplinaRepository disciplinas,
    IImportLogRepository importLogs,
    IUnitOfWork uow,
    IOptions<CsvImportOptions> options,
    ILogger<CsvImportService> logger) : ICsvImportService
{
    private static readonly string[] CabecalhoEsperado = ["codigo", "nome", "professor", "periodo", "creditos"];

    public async Task<ResultadoImportacaoCsv> ImportarAsync(CancellationToken ct = default)
    {
        var caminho = LocalizarArquivo();
        if (caminho is null)
        {
            logger.LogInformation("Importação de CSV: nenhum arquivo encontrado — etapa ignorada.");
            return new ResultadoImportacaoCsv(false, null, 0, 0, 0, "Arquivo não encontrado.");
        }

        string[] linhas;
        try
        {
            linhas = await File.ReadAllLinesAsync(caminho, System.Text.Encoding.UTF8, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Importação de CSV: falha ao ler {Arquivo}.", caminho);
            return new ResultadoImportacaoCsv(false, caminho, 0, 0, 0, $"Falha de leitura: {ex.Message}");
        }

        if (linhas.Length == 0)
            return new ResultadoImportacaoCsv(false, caminho, 0, 0, 0, "Arquivo vazio.");

        var cabecalho = CsvLineParser.Parse(RemoverBom(linhas[0])).Select(c => c.Trim().ToLowerInvariant()).ToArray();
        if (!CabecalhoEsperado.SequenceEqual(cabecalho))
        {
            logger.LogWarning("Importação de CSV: cabeçalho inesperado em {Arquivo}: {Cabecalho}", caminho, linhas[0]);
            return new ResultadoImportacaoCsv(false, caminho, 0, 0, 0,
                $"Cabeçalho inválido. Esperado: {string.Join(",", CabecalhoEsperado)}");
        }

        var codigosExistentes = new HashSet<string>(await disciplinas.ListarCodigosAsync(ct), StringComparer.OrdinalIgnoreCase);
        int importados = 0, ignorados = 0, invalidas = 0;

        foreach (var linha in linhas.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(linha))
                continue;

            var campos = CsvLineParser.Parse(linha);
            if (campos.Count != 5
                || string.IsNullOrWhiteSpace(campos[0])
                || string.IsNullOrWhiteSpace(campos[1])
                || !int.TryParse(campos[3], out var periodo)
                || !int.TryParse(campos[4], out var creditos))
            {
                invalidas++;
                continue;
            }

            var codigo = campos[0].Trim();
            if (codigosExistentes.Contains(codigo))
            {
                ignorados++; // duplicado: já existe no banco ou no próprio arquivo
                continue;
            }

            try
            {
                var disciplina = new Disciplina(codigo, campos[1], campos[2], periodo, creditos);
                await disciplinas.AdicionarAsync(disciplina, ct);
                codigosExistentes.Add(codigo);
                importados++;
            }
            catch (Domain.Exceptions.DomainException)
            {
                invalidas++; // valores fora das regras de domínio
            }
        }

        var hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(string.Join("\n", linhas))));
        await importLogs.AdicionarAsync(new ImportLog(System.IO.Path.GetFileName(caminho), hash, importados, ignorados, invalidas), ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation(
            "Importação de CSV concluída ({Arquivo}): {Importados} importadas, {Ignorados} ignoradas (duplicadas), {Invalidas} linhas inválidas.",
            caminho, importados, ignorados, invalidas);

        return new ResultadoImportacaoCsv(true, caminho, importados, ignorados, invalidas, null);
    }

    private string? LocalizarArquivo()
    {
        var opt = options.Value;

        if (!string.IsNullOrWhiteSpace(opt.Path))
            return File.Exists(opt.Path) ? opt.Path : null;

        var dir = opt.Directory;
        if (!System.IO.Directory.Exists(dir))
            return null;

        return opt.FileNames
            .Select(nome => System.IO.Path.Combine(dir, nome))
            .FirstOrDefault(File.Exists);
    }

    private static string RemoverBom(string s) => s.TrimStart('﻿');
}
