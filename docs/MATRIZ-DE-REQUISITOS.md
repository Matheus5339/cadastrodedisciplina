# Matriz de Requisitos

Origens: **N** = `instrucoes.txt` (normativo) · **R** = README original · **L** = `leia.txt` (roteiro) · **P** = documentos do professor (ausentes — requisitos genéricos herdados via L)

> **Evidências (2026-06-12):** backend `dotnet build` 0 erros/0 avisos e `dotnet test` **57/57 aprovados**; frontend `tsc --noEmit` exit 0; validação de runtime com API real (health Healthy, login demo, 72 disciplinas importadas do CSV, filtros, CR, rotação de refresh token, logout) e integração fim a fim via proxy do Vite. Detalhes em `RASTREAMENTO.md`.

## Backend

| ID | Requisito | Origem | Status |
| :--- | :--- | :--- | :--- |
| RB01 | Cadastro de aluno (autocadastro, e-mail válido, sem checagem de existência) | N§2, L§13 | Atendido |
| RB02 | Login com JWT Bearer de expiração curta | R, L§12 | Atendido |
| RB03 | Refresh token com rotação e revogação | L§12 | Atendido |
| RB04 | Logout com invalidação do refresh token | L§12 | Atendido |
| RB05 | Consulta e atualização dos dados do aluno | L§13 | Atendido |
| RB06 | Upload e consulta da foto (BLOB SQLite, limite de tamanho, validação de tipo) | N§3, L§12 | Atendido |
| RB07 | CRUD de disciplinas | N§2 | Atendido |
| RB08 | Filtros por nome, professor, semestre e/ou ano | N§2 | Atendido |
| RB09 | CRUD de histórico | N§3, L§13 | Atendido |
| RB10 | Cálculo do CR (média ponderada por créditos — hipótese D4) | N§2, L§16 | Atendido |
| RB11 | Isolamento por aluno; aluno identificado pelo token; `idAluno` do frontend nunca confiado | N§2, L§12 | Atendido |
| RB12 | Importação de CSV somente na inicialização, caminho configurável, sem duplicatas | N§2, L§15 | Atendido |
| RB13 | Endpoint `/health` sem informações sensíveis | R, L§12 | Atendido |
| RB14 | Swagger somente em desenvolvimento | L§10/§12 | Atendido |
| RB15 | Hash de senha com Argon2id (MD5/SHA-256 simples proibidos) | L§12 | Atendido |
| RB16 | Segredos fora do repositório (env vars / user secrets / config local) | R, L§12 | Atendido |
| RB17 | CORS restritivo configurável | R, L§12 | Atendido |
| RB18 | Validação de entrada e tratamento padronizado de erros | L§12 | Atendido |
| RB19 | Arquitetura em camadas `Api/Application/Domain/Infrastructure/Shared/Tests` (.NET 10, EF Core, SQLite, migrations, DI, DTOs, logs) | N§6, L§10 | Atendido |
| RB20 | Modelo de dados: Disciplina, Aluno, Histórico (+ RefreshTokens, ImportLog — D7) | N§3, L§11 | Atendido |

## Frontend

| ID | Requisito | Origem | Status |
| :--- | :--- | :--- | :--- |
| RF01 | React + Vite + TypeScript + Tailwind + shadcn/ui nas versões do N§4 | N§4, L§14 | Atendido |
| RF02 | Estrutura física conforme N§5 | N§5 | Atendido |
| RF03 | Tela de login | L§14, P | Atendido |
| RF04 | Tela de cadastro | N§2, L§14 | Atendido |
| RF05 | Layout autenticado + painel principal | L§14 | Atendido |
| RF06 | Listagem de disciplinas com filtros (nome, professor, semestre, ano) | N§2, L§14 | Atendido |
| RF07 | Formulário de disciplina + confirmação de exclusão | L§14 | Atendido |
| RF08 | Histórico acadêmico + formulário de histórico | L§14 | Atendido |
| RF09 | Exibição do CR | N§2, L§14 | Atendido |
| RF10 | Perfil do aluno + upload da foto | L§14 | Atendido |
| RF11 | Logout | L§14 | Atendido |
| RF12 | Páginas de erro e de acesso não autorizado | L§14 | Atendido |
| RF13 | Loading, empty state, alertas, tratamento visual de erros | L§14 | Atendido |
| RF14 | Validação de formulário (Zod), rotas protegidas, interceptadores, renovação de sessão, armazenamento seguro | N§5, L§14 | Atendido |
| RF15 | Responsividade, ícones, aparência organizada | L§14, P | Atendido |

## Testes

| ID | Requisito | Origem | Status |
| :--- | :--- | :--- | :--- |
| RT01 | Testes do CR: vazio, sem créditos, 1 disciplina, várias, decimais, arredondamento, isolamento | L§16 | Atendido |
| RT02 | Testes do CSV: ausente, válido, linha inválida, duplicidade, repetição, separador, caminho configurável | L§15 | Atendido |
| RT03 | Testes unitários (Domain/Application) e de integração (API/banco) | L§10/§17 | Atendido |
| RT04 | Build e testes executados de verdade após cada etapa, resultado real registrado | L§17 | Atendido |

## Entrega

| ID | Requisito | Origem | Status |
| :--- | :--- | :--- | :--- |
| RE01 | Sem `bin`, `obj`, `node_modules`, `dist`, `test-results`, executáveis, banco com dados pessoais, segredos ou tokens | L§18, P | Atendido |
| RE02 | Credenciais de demonstração registradas no README de entrega | L§18, P | Atendido |
| RE03 | Instruções de execução, teste e estrutura de diretórios documentadas | L§18 | Atendido |
| RE04 | `docs/ENTREGA.md` com participantes, RGU, credenciais, assunto do e-mail, checklist e instruções do ZIP | L§18 | Atendido (campos de participantes/e-mail a preencher pelo aluno — P7) |
