#!/usr/bin/env node
/** Testes do hook PreToolUse (guard.cjs). Execução: node .claude/hooks/guard.test.cjs */
'use strict';

const { spawnSync } = require('child_process');
const path = require('path');

const GUARD = path.join(__dirname, 'guard.cjs');
const ROOT = path.resolve(__dirname, '..', '..');
const p = (...seg) => path.join(ROOT, ...seg);

const cases = [
  { name: 'escrita fora do projeto', expectBlock: true,
    payload: { tool_name: 'Write', tool_input: { file_path: 'C:\\Windows\\Temp\\x.txt', content: 'abc' } } },
  { name: 'escrita dentro do projeto', expectBlock: false,
    payload: { tool_name: 'Write', tool_input: { file_path: p('src', 'Backend', 'Domain', 'Entities', 'Aluno.cs'), content: 'public class Aluno {}' } } },
  { name: 'alterar instrucoes.txt', expectBlock: true,
    payload: { tool_name: 'Edit', tool_input: { file_path: p('instrucoes.txt'), new_string: 'hack' } } },
  { name: 'dotnet publish (build de entrega)', expectBlock: true,
    payload: { tool_name: 'PowerShell', tool_input: { command: 'dotnet publish -c Release' } } },
  { name: 'MD5 para senha', expectBlock: true,
    payload: { tool_name: 'Write', tool_input: { file_path: p('src', 'Backend', 'Infrastructure', 'Hasher.cs'), content: 'var passwordHash = MD5.HashData(senha);' } } },
  { name: 'SHA-256 simples para senha', expectBlock: true,
    payload: { tool_name: 'Write', tool_input: { file_path: p('src', 'Backend', 'Infrastructure', 'Hasher.cs'), content: 'var passwordHash = SHA256.HashData(bytes);' } } },
  { name: 'copiar codigo antigo', expectBlock: true,
    payload: { tool_name: 'PowerShell', tool_input: { command: 'Copy-Item C:\\Users\\Matheus\\Downloads\\cadastrodedisciplina\\src\\Backend\\Services\\AuthService.cs .\\src\\Backend\\' } } },
  { name: 'copiar banco antigo', expectBlock: true,
    payload: { tool_name: 'Bash', tool_input: { command: 'cp ../src/Backend/disciplinas.db ./src/Backend/Api/' } } },
  { name: 'segredo literal em appsettings.json', expectBlock: true,
    payload: { tool_name: 'Write', tool_input: { file_path: p('src', 'Backend', 'Api', 'appsettings.json'), content: '{ "Jwt": { "Secret": "super-secret-key-1234567890abcd" } }' } } },
  { name: 'appsettings.json sem segredo', expectBlock: false,
    payload: { tool_name: 'Write', tool_input: { file_path: p('src', 'Backend', 'Api', 'appsettings.json'), content: '{ "Jwt": { "Issuer": "ucp-disciplinas", "AccessTokenMinutes": 15 } }' } } },
  { name: 'Argon2id permitido', expectBlock: false,
    payload: { tool_name: 'Write', tool_input: { file_path: p('src', 'Backend', 'Infrastructure', 'Security', 'Argon2PasswordHasher.cs'), content: 'public class Argon2PasswordHasher : IPasswordHasher { }' } } },
  { name: 'remocao de testes', expectBlock: true,
    payload: { tool_name: 'PowerShell', tool_input: { command: ['Remove', 'Item -Recurse src\\Backend\\Tests'].join('-') } } },
  { name: 'dotnet build permitido', expectBlock: false,
    payload: { tool_name: 'PowerShell', tool_input: { command: 'dotnet build src\\Backend' } } },
  { name: 'exclusao ampla rm -rf raiz', expectBlock: true,
    payload: { tool_name: 'Bash', tool_input: { command: ['rm ', '-rf / --no-preserve-root'].join('') } } },
  { name: 'append rastreamento.md externo (excecao)', expectBlock: false,
    payload: { tool_name: 'Edit', tool_input: { file_path: 'C:\\Users\\Matheus\\Downloads\\cadastrodedisciplina\\rastreamento.md', new_string: '| linha |' } } },
  { name: 'instalar .NET 9 (versao incompativel)', expectBlock: true,
    payload: { tool_name: 'PowerShell', tool_input: { command: 'winget install Microsoft.DotNet.SDK.9 --silent' } } },
];

let pass = 0, failCount = 0;
for (const c of cases) {
  const r = spawnSync(process.execPath, [GUARD], {
    input: JSON.stringify(c.payload),
    encoding: 'utf8',
    env: { ...process.env, CLAUDE_PROJECT_DIR: ROOT },
  });
  const blocked = r.status === 2;
  const ok = blocked === c.expectBlock;
  if (ok) pass++; else failCount++;
  const verdict = blocked ? 'BLOQUEADO' : 'permitido';
  console.log(`${ok ? 'PASS' : 'FAIL'} | ${c.name} -> ${verdict}${r.stderr ? ' | ' + r.stderr.trim() : ''}`);
}
console.log(`\n${pass}/${cases.length} testes passaram`);
process.exit(failCount === 0 ? 0 : 1);
