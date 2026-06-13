# Controle de Disciplinas — UCP (Projeto Novo)

<p align="center">
  <img alt=".NET 10"      src="https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white">
  <img alt="C#"           src="https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white">
  <img alt="React 19"     src="https://img.shields.io/badge/React-19.2-61DAFB?logo=react&logoColor=black">
  <img alt="Vite 8"       src="https://img.shields.io/badge/Vite-8.0-646CFF?logo=vite&logoColor=white">
  <img alt="TypeScript 6" src="https://img.shields.io/badge/TypeScript-6.0-3178C6?logo=typescript&logoColor=white">
  <img alt="Tailwind 4"   src="https://img.shields.io/badge/Tailwind-4.3-06B6D4?logo=tailwindcss&logoColor=white">
  <img alt="SQLite"       src="https://img.shields.io/badge/SQLite-3.53-003B57?logo=sqlite&logoColor=white">
  <img alt="Testes"       src="https://img.shields.io/badge/testes-57%2F57%20aprovados-3FB950">
</p>

Sistema completo (frontend + backend + SQLite) para controlar as disciplinas cursadas por alunos do curso de Engenharia da Computação da UCP, com autenticação, autorização, isolamento por aluno e cálculo do CR.

> **Status:** Fases 0–4 concluídas. Backend com **57/57 testes aprovados**, frontend com typecheck limpo e integração validada fim a fim. Instruções de execução e entrega em [`docs/ENTREGA.md`](./docs/ENTREGA.md).

## Funcionalidades

- 🔐 **Autenticação** com Argon2id + JWT de curta duração e refresh token com rotação/revogação
- 👤 **Isolamento por aluno** — cada usuário acessa somente as próprias disciplinas e histórico
- 📚 **CRUD de disciplinas** com filtros por nome, professor, ano e semestre
- 🧮 **Cálculo do CR** ponderado por créditos
- 🗂️ **Importação de CSV** de disciplinas na inicialização (caminho configurável)
- 🖼️ **Foto do aluno** armazenada como BLOB (limite de 2 MB)
- ❤️ **Health check** (`/health`) e **Swagger** (somente em desenvolvimento)

## Execução rápida

```bash
# Backend (requer SDK .NET 10) — porta 5080
cd src/Backend
dotnet run --project Api

# Frontend (requer Node.js 24.x) — porta 5173
cd src/Frontend
npm install
npm run dev
```

Credenciais de demonstração e checklist completo em [`docs/ENTREGA.md`](./docs/ENTREGA.md).

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
