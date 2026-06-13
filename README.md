# Controle de Disciplinas — UCP (Projeto Novo)

Sistema completo (frontend + backend + SQLite) para controlar as disciplinas cursadas por alunos do curso de Engenharia da Computação da UCP, com autenticação, autorização, isolamento por aluno e cálculo do CR.

> **Status:** Fases 0–4 concluídas. Backend com **57/57 testes aprovados**, frontend com typecheck limpo e integração validada fim a fim. Instruções de execução e entrega em [`docs/ENTREGA.md`](./docs/ENTREGA.md).

## Fonte normativa

O arquivo [`instrucoes.txt`](./instrucoes.txt) é a regra superior deste projeto. Ordem de precedência completa em [`docs/REGRAS-COMPLEMENTARES.md`](./docs/REGRAS-COMPLEMENTARES.md).

## Tecnologias

- **Backend:** .NET 10 (C#), Entity Framework Core, SQLite, JWT Bearer + refresh token, Argon2id
- **Frontend:** React 19.2.6, Vite 8.0.14, Tailwind 4.3.0, shadcn 3.2.1, TypeScript 6.0.2, Node.js 24.16.0

## Estrutura planejada

```
controle-disciplinas-ucp-novo/
├── instrucoes.txt              # normativo (somente leitura)
├── CLAUDE.md                   # governança do agente
├── README.md / TAREFAS.md / RASTREAMENTO.md / DECISOES.md / PENDENCIAS.md
├── .claude/                    # regras + hook PreToolUse de governança
├── docs/                       # análise de referências, matriz de requisitos, entrega
└── src/
    ├── Backend/
    │   ├── Api/                # controllers, contracts, middlewares, Program.cs
    │   ├── Application/        # casos de uso, DTOs, validadores, interfaces
    │   ├── Domain/             # entidades, value objects, regras puras
    │   ├── Infrastructure/     # EF Core, SQLite, JWT, Argon2id, CSV, foto
    │   ├── Shared/             # Result, erros, constantes, helpers
    │   └── Tests/              # unitários + integração
    └── Frontend/               # projeto Vite (árvore interna conforme instrucoes.txt §5)
```

## Execução (após implementação)

```bash
# Backend (requer SDK .NET 10)
cd src/Backend
dotnet run --project Api

# Frontend (requer Node.js 24.x)
cd src/Frontend
npm install
npm run dev
```

Endpoints de apoio: `/health` (health check) e `/swagger` (somente em desenvolvimento).

## Segurança

- Senhas com **Argon2id** (MD5 e SHA-256 simples proibidos)
- JWT de curta duração + refresh token com rotação e revogação
- Segredos via variáveis de ambiente / user secrets — **nunca no repositório**
- CORS restritivo, validação de entrada, isolamento por aluno via token

## Documentação

- [`docs/ANALISE-DE-REFERENCIAS.md`](./docs/ANALISE-DE-REFERENCIAS.md) — análise das fontes e conflitos
- [`docs/MATRIZ-DE-REQUISITOS.md`](./docs/MATRIZ-DE-REQUISITOS.md) — requisitos rastreáveis
- [`DECISOES.md`](./DECISOES.md) — decisões com justificativa
- [`PENDENCIAS.md`](./PENDENCIAS.md) — pendências que exigem decisão humana
- [`RASTREAMENTO.md`](./RASTREAMENTO.md) — registro cronológico das ações
