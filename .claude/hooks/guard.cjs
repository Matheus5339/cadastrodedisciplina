#!/usr/bin/env node
/**
 * Hook PreToolUse — governança do projeto controle-disciplinas-ucp-novo.
 *
 * Lê o payload JSON do Claude Code via stdin e decide:
 *   - exit 0  -> ação permitida
 *   - exit 2  -> ação BLOQUEADA (mensagem no stderr volta para o agente)
 *
 * Bloqueia:
 *   1. escrita fora de controle-disciplinas-ucp-novo;
 *   2. exclusões amplas (rm -rf /, Remove-Item -Recurse em raízes, del /s etc.);
 *   3. alterações em instrucoes.txt;
 *   4. cópia de código-fonte/bancos das implementações antigas;
 *   5. MD5 ou SHA-256 simples para armazenar senhas;
 *   6. segredos gravados em arquivos versionados;
 *   7. remoção de testes;
 *   8. geração de builds para entrega (dotnet publish / vite build);
 *   9. instalação silenciosa de versões incompatíveis (.NET != 10).
 */
'use strict';

const path = require('path');

function fail(msg) {
  process.stderr.write(`[governanca] BLOQUEADO: ${msg}\n`);
  process.exit(2);
}

function readStdin() {
  return new Promise((resolve) => {
    let data = '';
    process.stdin.setEncoding('utf8');
    process.stdin.on('data', (c) => (data += c));
    process.stdin.on('end', () => resolve(data));
  });
}

const PROJECT_DIR_NAME = 'controle-disciplinas-ucp-novo';

function projectRoot() {
  const env = process.env.CLAUDE_PROJECT_DIR;
  if (env && env.toLowerCase().includes(PROJECT_DIR_NAME)) return path.resolve(env);
  // fallback: este script vive em <root>/.claude/hooks/
  return path.resolve(__dirname, '..', '..');
}

function isInsideProject(p) {
  const abs = path.resolve(projectRoot());
  const target = path.resolve(p);
  const rel = path.relative(abs, target);
  return rel === '' || (!rel.startsWith('..') && !path.isAbsolute(rel));
}

// --- validações de conteúdo -------------------------------------------------

function checkContent(filePath, content) {
  const lowerPath = (filePath || '').toLowerCase();
  const text = content || '';

  // MD5 / SHA-256 simples para senhas
  const weakHash = /\b(md5|sha-?1|sha-?256)\b/i;
  const passwordCtx = /(senha|password|passwordhash|hashpassword|hashsenha)/i;
  if (weakHash.test(text) && passwordCtx.test(text)) {
    // permitido apenas em documentação que explica a proibição
    const isDoc = /\.(md|txt)$/i.test(lowerPath);
    const mentionsBan = /(proibid|n[aã]o (utiliz|us)|bloquei|forbidden|never)/i.test(text);
    if (!(isDoc && mentionsBan)) {
      fail(`uso de MD5/SHA-256 em contexto de senha em "${filePath}". Senhas devem usar Argon2id.`);
    }
  }

  // segredos literais em arquivos versionáveis
  const versionedConfig = /\.(json|xml|yaml|yml|cs|ts|tsx|js|env)$/i.test(lowerPath)
    && !/appsettings\.development\.json$/i.test(lowerPath)
    && !/\.env\.local$/i.test(lowerPath)
    && !/\.example/i.test(lowerPath);
  if (versionedConfig) {
    const secretLiteral = /("(secret|jwt[_:]?secret|pepper|password|connection_?string_?password)"\s*:\s*"[^"${}\s][^"]{11,}")|((SecretKey|Pepper|PrivateKey)\s*=\s*"[^"$]{12,}")/i;
    if (secretLiteral.test(text)) {
      fail(`possível segredo literal em arquivo versionado "${filePath}". Use variáveis de ambiente, user secrets ou arquivo local não versionado.`);
    }
  }
}

// --- validações de comandos shell --------------------------------------------

function checkCommand(cmd) {
  const c = cmd.toLowerCase();

  // alterações em instrucoes.txt via shell
  if (/instrucoes\.txt/.test(c) && /(out-file|set-content|add-content|>\s*\S*instrucoes\.txt|>>|del |remove-item|rm |mv |move-item|rename)/.test(c)) {
    fail('comando tenta alterar/remover instrucoes.txt (arquivo normativo, somente leitura).');
  }

  // exclusões amplas
  const broadDelete = [
    /rm\s+(-[a-z]*r[a-z]*f|-[a-z]*f[a-z]*r)[a-z]*\s+(\/|~|\\|[a-z]:\\?\s|\*|\.\s*$|\.\.\s*)/i,
    /remove-item\b.*-recurse\b.*(\b[a-z]:\\(?![^ ]*controle-disciplinas-ucp-novo)|\\\*|\s\/\s|\s~\s)/i,
    /rmdir\s+\/s/i,
    /del\s+\/[sf]/i,
    /format\s+[a-z]:/i,
  ];
  if (broadDelete.some((re) => re.test(c))) {
    fail('exclusão ampla/destrutiva detectada. Exclusões devem ser pontuais e dentro do projeto novo.');
  }

  // remoção de testes
  if (/(remove-item|rm |del |rmdir|git\s+rm)\b.*(tests?|\.tests|test-)/i.test(c)) {
    fail('remoção de testes detectada. Testes não podem ser removidos.');
  }

  // cópia de código/banco antigos para o projeto novo
  const oldSources = /(cadastrodedisciplina\\src|cadastrodedisciplina\/src|disciplinas\.db|\.zip)/i;
  if (/(copy-item|copy |cp |xcopy|robocopy|move-item|move |expand-archive|unzip|tar )/i.test(c) && oldSources.test(c)) {
    fail('cópia de código-fonte/banco/ZIP de implementações anteriores é proibida (somente leitura como referência).');
  }

  // builds de entrega
  if (/(dotnet\s+publish|npm\s+run\s+build|pnpm\s+(run\s+)?build|vite\s+build|dotnet\s+pack)/i.test(c)) {
    fail('geração de build para entrega é proibida (entrega contém somente código-fonte).');
  }

  // instalação de versões incompatíveis de .NET
  const dotnetInstall = c.match(/(winget\s+install[^;|&]*microsoft\.dotnet[^;|&]*|dotnet-install[^;|&]*)/i);
  if (dotnetInstall && /\b([1-9]|net9|9\.0|8\.0|7\.0|6\.0)\b/.test(dotnetInstall[0]) && !/10/.test(dotnetInstall[0])) {
    fail('instalação de versão do .NET diferente da exigida (.NET 10) requer autorização humana.');
  }

  // escrita via redirecionamento fora do projeto
  const redirect = c.match(/(?:>>?|out-file\s+(?:-filepath\s+)?|set-content\s+(?:-path\s+)?|add-content\s+(?:-path\s+)?)\s*"?([a-z]:\\[^"\s>|;]+)/i);
  if (redirect && !redirect[1].toLowerCase().includes(PROJECT_DIR_NAME) && !redirect[1].toLowerCase().includes('rastreamento.md')) {
    fail(`escrita fora do projeto novo detectada no comando ("${redirect[1]}").`);
  }
}

// --- main --------------------------------------------------------------------

(async () => {
  let payload;
  try {
    // remove BOM e espaços — PowerShell 5.1 envia BOM UTF-8 no pipe
    payload = JSON.parse((await readStdin()).replace(/^﻿/, '').trim());
  } catch {
    // payload ilegível: não bloquear (fail-open) para não travar a sessão
    process.exit(0);
  }

  const tool = payload.tool_name || '';
  const input = payload.tool_input || {};

  if (['Write', 'Edit', 'NotebookEdit'].includes(tool)) {
    const filePath = input.file_path || input.notebook_path || '';
    if (filePath) {
      const lower = filePath.toLowerCase().replace(/\//g, '\\');

      // exceção única: append de rastreamento exigido pelo CLAUDE.md do diretório pai
      const isExternalRastreamento = /cadastrodedisciplina\\rastreamento\.md$/i.test(lower);

      if (!isInsideProject(filePath) && !isExternalRastreamento) {
        fail(`escrita fora de ${PROJECT_DIR_NAME}: "${filePath}".`);
      }
      if (/instrucoes\.txt$/i.test(lower)) {
        fail('instrucoes.txt é o arquivo normativo e não pode ser modificado.');
      }
      if (/\.(db|sqlite|sqlite3)$/i.test(lower)) {
        fail('escrita direta de arquivos de banco é proibida; o banco é criado por migrations em runtime.');
      }
    }
    checkContent(filePath, (input.content || '') + ' ' + (input.new_string || ''));
  }

  if (['Bash', 'PowerShell'].includes(tool)) {
    checkCommand(String(input.command || ''));
  }

  process.exit(0);
})();
