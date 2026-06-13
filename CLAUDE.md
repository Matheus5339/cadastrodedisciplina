@./instrucoes.txt
@./docs/REGRAS-COMPLEMENTARES.md

# Controle de Disciplinas UCP — Projeto Novo

Sistema de controle de disciplinas cursadas (Engenharia da Computação — UCP), desenvolvido do zero.

## Fonte normativa

- `instrucoes.txt` (raiz deste projeto) é a **regra superior**. Nenhuma ação pode contrariá-lo.
- `docs/REGRAS-COMPLEMENTARES.md` define a governança operacional.
- `.claude/rules/governanca.md` define regras de execução para o Claude Code.

## Resumo técnico

- **Backend:** .NET 10 (C#), EF Core + SQLite, Clean Architecture em `src/Backend/{Api,Application,Domain,Infrastructure,Shared,Tests}`.
- **Frontend:** React 19.2.6, Vite 8.0.14, Tailwind 4.3.0, shadcn 3.2.1, TypeScript 6.0.2, Node.js 24.16.0, em `src/Frontend/` com a árvore definida em `instrucoes.txt` §5.
- **Segurança:** Argon2id para senhas (MD5 e SHA-256 simples são proibidos), JWT Bearer de curta duração + refresh token com rotação/revogação, segredos fora do repositório.
- **CR:** `CR = soma(mediaFinal × creditos) / soma(creditos)`, arredondado a 2 casas (hipótese registrada em `DECISOES.md`).
- **CSV:** importação somente na inicialização, caminho configurável (divergência de nomes documentada em `DECISOES.md`).

## Rastreamento

Toda interação deve ser registrada em `RASTREAMENTO.md` (data/hora, comando, arquivos afetados, resumo, resultado, falhas). Decisões em `DECISOES.md`, ambiguidades em `PENDENCIAS.md`, tarefas em `TAREFAS.md`.
