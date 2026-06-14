@./instrucoes-figurinhas.txt

# Álbum de Figurinhas — UCP (Projeto)

Sistema para simular um álbum de figurinhas virtual, conforme o trabalho do professor
(documentos "Trabalho Algum 2026-01" em `docs/referencias-professor/`).

## Fonte normativa

- **`instrucoes-figurinhas.txt`** (raiz deste projeto) é a **regra superior** do projeto. Nenhuma ação pode contrariá-lo.

## Resumo técnico

- **Backend:** .NET 10 (C#), EF Core + SQLite, Clean Architecture em `src/Backend/{Api,Application,Domain,Infrastructure,Shared,Tests}`.
- **Frontend:** React 19.2.6, Vite 8.0.14, Tailwind 4.3.0, shadcn 3.2.1, TypeScript 6.0.2, Node.js 24.16.0, em `src/Frontend/` com a árvore definida em `instrucoes-figurinhas.txt` §7.
- **Domínio:** `Usuario` (login por nome + perfil Administrador/Autor/Colecionador), `Album` único, `Figurinha` (tag = MD5 da imagem), `FigurinhaAdquirida`.
- **Segurança:** Argon2id para senhas (MD5 e SHA-256 simples são proibidos para senhas); o MD5 é usado **somente** para a tag da imagem. JWT + refresh token httpOnly com rotação/revogação; segredos fora do repositório.

## Rastreamento

Toda interação deve ser registrada em `RASTREAMENTO.md` (data/hora, comando, arquivos afetados, resumo, resultado, falhas). Decisões em `DECISOES.md`, ambiguidades em `PENDENCIAS.md`, tarefas em `TAREFAS.md`.
