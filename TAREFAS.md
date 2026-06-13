# Tarefas

| ID | Tarefa | Requisito associado | Status | Evidência | Data de conclusão |
| :--- | :--- | :--- | :--- | :--- | :--- |
| T01 | Ler referências (instrucoes.txt, README, leia.txt, CSVs, implementação antiga) | L§5 | Concluída | `docs/ANALISE-DE-REFERENCIAS.md` | 2026-06-11 |
| T02 | Buscar documentos do professor e ZIPs | L§5 | Concluída (ausentes) | `PENDENCIAS.md` P2/P3 | 2026-06-11 |
| T03 | Criar pasta `controle-disciplinas-ucp-novo` e copiar referências | L§6 | Concluída | `instrucoes.txt`, `docs/referencias/README-ORIGINAL.md` | 2026-06-11 |
| T04 | Criar governança (CLAUDE.md, REGRAS-COMPLEMENTARES.md, governanca.md) | L§7 | Concluída | arquivos na raiz e em `.claude/rules/` | 2026-06-11 |
| T05 | Configurar e validar hook PreToolUse | L§8 | Concluída | `.claude/hooks/guard.cjs` + `guard.test.cjs` (16/16 PASS) | 2026-06-11 |
| T06 | Criar arquivos de controle (README, TAREFAS, RASTREAMENTO, DECISOES, PENDENCIAS, .gitignore) | L§9 | Concluída | arquivos na raiz | 2026-06-11 |
| T07 | Criar análise de referências e matriz de requisitos | L§5/§9 | Concluída | `docs/ANALISE-DE-REFERENCIAS.md`, `docs/MATRIZ-DE-REQUISITOS.md` | 2026-06-11 |
| T08 | Obter autorização humana para implementar (Fase 1+) | L§21 | Concluída | Usuário autorizou: "1a, 2a, 3a, 4a" | 2026-06-12 |
| T09 | Instalar SDK .NET 10 (depende de P1) | L§10 | Concluída | `dotnet --list-sdks` → 10.0.301 (em `%LOCALAPPDATA%\Microsoft\dotnet`) | 2026-06-12 |
| T10 | Fase 1 — estrutura física, solução, projetos, dependências | L§19 | Concluída | 6 projetos (.slnx), camadas físicas, `dotnet restore` OK, dotnet-ef local | 2026-06-12 |
| T11 | Fase 2 — Domain, Application, Infrastructure, Api, segurança, testes | L§19 | Concluída | Build 0 erros/0 avisos; migration `Inicial`; **57/57 testes aprovados** (`dotnet test`) | 2026-06-12 |
| T12 | Fase 3 — frontend completo e integração | L§19 | Concluída | ~60 arquivos (árvore §5); `tsc --noEmit` exit 0; integração validada via proxy (registro→histórico→CR 8,5→refresh→logout) | 2026-06-12 |
| T13 | Fase 4 — testes completos, correções, documentação, entrega limpa | L§19 | Concluída | `docs/ENTREGA.md` com credenciais demo, instruções e script de ZIP; banco local removido; matriz atualizada | 2026-06-12 |
