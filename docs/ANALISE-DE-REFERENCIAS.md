# Análise de Referências

Data: 2026-06-11
Responsável: Claude Code (Fase 0 — leitura de referências)

## 1. Arquivos lidos

| Arquivo | Papel | Situação |
| :--- | :--- | :--- |
| `../instrucoes.txt` | Fonte normativa superior | Lido integralmente (175 linhas) e copiado para a raiz do projeto novo |
| `../README.md` | Fonte técnica complementar | Lido integralmente e copiado para `docs/referencias/README-ORIGINAL.md` |
| `../CLAUDE.md` | Governança do projeto antigo | Lido (referência de aprendizado) |
| `../leia.txt` | Roteiro de execução deste trabalho | Lido integralmente |
| `../disciplinas-ecomp-ucp.csv` | Dados de disciplinas | Inspecionado (cabeçalho: `codigo,nome,professor,periodo,creditos`) |
| `../historico-aluno.csv` | Dados de histórico | Inspecionado (cabeçalho: `disciplinaCodigo,disciplinaNome,ano,semestre,periodo,mediaFinal`) |
| `../src/Backend/**` | Implementação anterior | Inspecionada estrutura e pontos críticos — **nenhum código copiado** |
| `../src/Frontend/**` | Implementação anterior | Inspecionada estrutura de diretórios — **nenhum código copiado** |
| `Trabalho Algum 2026-01.pdf` | Documento do professor | **NÃO ENCONTRADO** no diretório (ver `PENDENCIAS.md`) |
| `Trabalho Algum 2026-01.rtf.doc` | Documento do professor | **NÃO ENCONTRADO** no diretório (ver `PENDENCIAS.md`) |
| `Trabalho Algum 2026-01.txt` | Documento do professor | **NÃO ENCONTRADO** no diretório (ver `PENDENCIAS.md`) |
| Arquivos `*.zip` | Implementações anteriores | **NENHUM ZIP existe** no diretório (busca recursiva executada) |

## 2. Regras obrigatórias (de `instrucoes.txt`)

1. Sistema de controle de disciplinas cursadas por um único aluno (multiusuário, cada usuário vê só os próprios dados).
2. CRUD de disciplina; listagem e filtro por nome, professor, semestre e/ou ano.
3. Exibição do CR (coeficiente de rendimento) — **fórmula não especificada** (ver conflitos).
4. Controle de acesso com isolamento total por usuário.
5. Autocadastro com e-mail válido (sem verificação de existência).
6. Importação do CSV de disciplinas no início da aplicação, a partir de diretório específico.
7. Modelo de dados mínimo: `disciplina(id, código, nome, professor, período, créditos)`, `aluno(id, rgu, cpf, email, nome, foto)`, `historico(id, idAluno, idDisciplina, ano, semestre, período, médiaFinal)`.
8. Tecnologias: React 19.2.6, Vite 8.0.14, Tailwind 4.3.0, shadcn 3.2.1, TypeScript 6.0.2, Node.js 24.16.0; backend "corenet 10 C#" (.NET 10); SQLite 3.53.1.
9. Estrutura física do frontend (§5) e do backend (§6) obrigatórias.

## 3. Regras complementares (do README original e do `leia.txt`)

- Clean Architecture: Api / Application / Domain / Infrastructure / Shared / Tests.
- EF Core + SQLite; migrations.
- JWT Bearer; refresh token com rotação e revogação; Argon2id para senhas.
- Segredos via variáveis de ambiente / user secrets / configuração local não versionada.
- CORS restritivo via configuração; tratamento global de exceções; health check em `/health`; Swagger somente em desenvolvimento.
- Foto do aluno como BLOB no SQLite, com limite de tamanho e validação de tipo.
- Testes unitários e de integração obrigatórios; resultados nunca inventados.

## 4. Conflitos encontrados e resolução pela precedência

| # | Conflito | Fontes | Resolução |
| :--- | :--- | :--- | :--- |
| C1 | Versão do .NET: 10 vs 9 | `instrucoes.txt` ("corenet 10") vs README original/CLAUDE.md antigo (.NET 9) | **.NET 10** (precedência 1) |
| C2 | Nome do CSV: `discipolinas.csv` (typo em `instrucoes.txt`) vs `disciplinas.csv` (CLAUDE.md antigo) vs `disciplinas-ecomp-ucp.csv` (arquivo real existente) | instrucoes/CLAUDE.md/arquivos reais | Caminho **configurável**, padrão `disciplinas.csv`, com aliases documentados (`discipolinas.csv`, `disciplinas-ecomp-ucp.csv`) — ver `DECISOES.md` D3 |
| C3 | Fórmula do CR não escrita em nenhum documento | — | Hipótese: média ponderada por créditos, registrada em `DECISOES.md` D4 |
| C4 | Hash de senha: CLAUDE.md antigo diz "SHA-256 + salt", mas o código antigo usa Argon2id (Isopoh); `leia.txt` proíbe MD5 e SHA-256 simples | CLAUDE.md antigo vs código antigo vs leia.txt | **Argon2id** (documentação antiga estava inconsistente com o próprio código) |
| C5 | Backend antigo é projeto único achatado (`src/Backend/*.cs` em pastas planas) vs arquitetura em camadas exigida | implementação antiga vs `instrucoes.txt` §6 | **Camadas físicas** `src/Backend/{Api,Application,Domain,Infrastructure,Shared,Tests}` |
| C6 | `instrucoes.txt` §5/§6 descrevem as árvores a partir de `src/`; o repositório precisa abrigar frontend e backend juntos | instrucoes vs leia.txt §10 | Backend em `src/Backend/<camadas>` (mandado explícito do roteiro); frontend em `src/Frontend/` com projeto Vite cuja pasta `src/` interna segue exatamente a árvore do §5 — ver `DECISOES.md` D5 |
| C7 | Modelo `aluno` em `instrucoes.txt` não tem `passwordHash`, mas autenticação exige | instrucoes vs requisitos de segurança | Adicionar `passwordHash` + campos técnicos mínimos (decisão D6); tabela técnica de refresh tokens documentada |

## 5. Decisões aplicadas

Ver `DECISOES.md` (D1–D8). Resumo: .NET 10; EF Core + SQLite; Argon2id; JWT curto + refresh token com rotação; CSV configurável; CR como média ponderada (hipótese); frontend Vite em `src/Frontend`; tabelas técnicas adicionais (RefreshTokens, ImportLog).

## 6. Dúvidas restantes

Ver `PENDENCIAS.md`. Principais: SDK .NET não instalado na máquina (nenhuma versão); documentos do professor ausentes; ZIPs inexistentes; disponibilidade exata das versões de pacotes npm exigidas.

## 7. Itens descartados por pertencerem ao álbum virtual

Os documentos do professor (`Trabalho Algum 2026-01.*`) não foram encontrados, portanto **nenhum requisito específico do álbum de figurinhas foi lido nem transportado**. Permanece a regra preventiva: qualquer funcionalidade típica de álbum (figurinhas, colagem, troca, pacotes etc.) está proibida neste sistema. Apenas requisitos genéricos compatíveis (banco embarcado, camadas, login, CRUD, listagem, filtros, ícones, documentação visual, entrega só de código-fonte, credenciais de teste, remoção de artefatos) foram incorporados via `leia.txt`.

## 8. Aprendizados da implementação anterior (sem cópia de código)

- O hasher antigo usava Argon2id com pepper configurável e falha rápida quando `Argon2:Pepper` ausente — padrão bom, será reimplementado do zero.
- SHA-256 era usado apenas para *hash de refresh token* (aceitável; não é hash de senha) — distinção mantida nas regras do hook.
- O projeto antigo continha `disciplinas.db` versionado e `dist/`, `test-results/` no frontend — erros de higiene que o novo `.gitignore` e a preparação de entrega evitam.
- O frontend antigo seguia a árvore do §5 apenas parcialmente (sem `components/feedback` completo etc.) — o novo seguirá integralmente.
