# Entrega Acadêmica — Controle de Disciplinas UCP

## 1. Participantes

| Nome | RGU |
| :--- | :--- |
| _(preencher)_ | _(preencher)_ |
| _(preencher)_ | _(preencher)_ |

## 2. E-mail de entrega

- **Destinatário:** _(preencher conforme orientação do professor)_
- **Assunto:** _(preencher conforme orientação do professor — ex.: "Trabalho 2026-01 — Controle de Disciplinas — <nomes>")_

## 3. Credenciais de demonstração

O aluno de demonstração é criado automaticamente na primeira execução em ambiente de desenvolvimento (configuração `Seed:DemoAluno: true` no `appsettings.Development.json`):

| Campo | Valor |
| :--- | :--- |
| E-mail | `demo@ucp.edu.br` |
| Senha | `Demo@123456` |
| RGU | `2026100001` |

Também é possível criar uma conta nova pela tela de cadastro (qualquer e-mail válido).

## 4. Como executar

### Pré-requisitos

- SDK **.NET 10**
- **Node.js 24.x** (testado com 24.16.0)

### Backend

```bash
cd src/Backend/Api
# 1) criar a configuração local de desenvolvimento (NÃO versionada):
#    copie appsettings.Development.example.json para appsettings.Development.json
#    e defina Jwt:Secret com um valor aleatório de 32+ caracteres.
cd ..
dotnet run --project Api
```

A API sobe em `http://localhost:5080` com:
- `/health` — health check;
- `/swagger` — documentação (somente em desenvolvimento);
- migrations aplicadas e CSV de disciplinas importado automaticamente na inicialização.

**CSV:** o importador procura, no diretório configurado (`CsvImport:Directory`, padrão `data/`), os nomes `disciplinas.csv`, `discipolinas.csv` e `disciplinas-ecomp-ucp.csv` (nesta ordem), ou usa o caminho explícito `CsvImport:Path`. Coloque seu CSV (cabeçalho `codigo,nome,professor,periodo,creditos`) em `src/Backend/Api/data/` ou aponte o `Path`.

### Frontend

```bash
cd src/Frontend
npm install
npm run dev
```

Acesse `http://localhost:5173` (o Vite faz proxy de `/api` para o backend).

## 5. Como testar

```bash
cd src/Backend
dotnet test            # 57 testes (unitários + integração)

cd ../Frontend
npm run typecheck      # checagem de tipos (tsc --noEmit)
```

## 6. Estrutura de diretórios

```
controle-disciplinas-ucp-novo/
├── instrucoes.txt            # arquivo normativo (somente leitura)
├── CLAUDE.md                 # governança de agente
├── README.md / TAREFAS.md / RASTREAMENTO.md / DECISOES.md / PENDENCIAS.md
├── docs/                     # análise de referências, matriz de requisitos, entrega
└── src/
    ├── Backend/              # .NET 10, Clean Architecture
    │   ├── Api/              # controllers, contracts, middlewares, filtros, Program
    │   ├── Application/      # casos de uso, DTOs, validadores, mapeamentos
    │   ├── Domain/           # entidades, value objects, CrCalculator, exceções
    │   ├── Infrastructure/   # EF Core+SQLite, migrations, JWT, Argon2id, CSV
    │   ├── Shared/           # kernel, constantes, extensões, helpers
    │   └── Tests/            # 57 testes (xunit + WebApplicationFactory)
    └── Frontend/             # React 19 + Vite 8 + TS 6 + Tailwind 4 + shadcn
        └── src/              # árvore conforme instrucoes.txt §5
```

## 7. Decisões relevantes

Ver `DECISOES.md` (D1–D14). Destaques: .NET 10 (precedência do normativo); Argon2id para senhas; JWT 15 min + refresh token 7 dias com rotação/revogação; CR = média ponderada por créditos (confirmada); CSV com caminho configurável e aliases documentados; disciplina como catálogo global com histórico isolado por aluno.

## 8. Checklist final de entrega

- [ ] Nomes e RGUs preenchidos acima
- [ ] Assunto/destinatário do e-mail preenchidos
- [x] Sem `bin/`, `obj/`, `node_modules/`, `dist/`, `test-results/` no ZIP (exclusões no script abaixo)
- [x] Sem executáveis no ZIP
- [x] Sem banco local (`*.db`) no ZIP
- [x] Sem segredos/tokens no ZIP (`appsettings.Development.json` excluído; só o `.example` é entregue)
- [x] Credenciais de demonstração documentadas
- [x] Instruções de execução e teste documentadas
- [x] Build e testes executados de verdade (resultados em `RASTREAMENTO.md`)

## 9. Gerar o ZIP de entrega

No PowerShell, a partir da pasta **pai** de `controle-disciplinas-ucp-novo`:

```powershell
$origem = "controle-disciplinas-ucp-novo"
$destino = "controle-disciplinas-ucp-entrega.zip"
$excluir = @("bin", "obj", "node_modules", "dist", "test-results", ".vs", "data")

$temp = Join-Path $env:TEMP "entrega-cdu"
Remove-Item $temp -Recurse -Force -ErrorAction SilentlyContinue
robocopy $origem $temp /E /XD $excluir /XF *.db *.db-shm *.db-wal appsettings.Development.json *.exe *.dll *.pdb | Out-Null
Compress-Archive -Path "$temp\*" -DestinationPath $destino -Force
Remove-Item $temp -Recurse -Force
Write-Host "ZIP gerado: $destino"
```

O ZIP resultante contém somente código-fonte, documentação e configurações de exemplo.
