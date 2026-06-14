# Entrega Acadêmica — Álbum de Figurinhas (UCP, P3 2026/01)

## 1. Participantes

| Nome | RGU |
| :--- | :--- |
| Matheus de Oliveira Pereira | 12310743 |
| Asafe da Silva Ferreira | 12310486 |

> Grupos de no máximo 3 pessoas (regra do enunciado).

## 2. E-mail de entrega (conforme `instrucoes-figurinhas.txt` §11)

- **Destinatário:** `mozar.silva@gmail.com`
- **Assunto:** `trab ucp P3 2026 01`
- **Data de entrega:** 15 de junho de 2026
- **Conteúdo do e-mail:**
  - RGU e nome de todos os participantes
  - ZIP com o **código-fonte** do projeto (**sem executáveis**)
  - **Login e senha** de acesso (ver abaixo)

## 3. Credenciais de acesso (seed inicial)

Criados automaticamente na primeira execução. Senha de todos: **`123456`**.

| Login | Perfil |
| :--- | :--- |
| `admin` | Administrador |
| `autor` | Autor |
| `colecionador` | Colecionador |

O administrador pode **zerar a senha** de qualquer usuário (volta para `123456`).

## 4. Como executar

### Pré-requisitos
- SDK **.NET 10** · **Node.js 24.x**

### Backend
```bash
cd src/Backend/Api
# crie a config local (NÃO versionada): copie appsettings.Development.example.json
# para appsettings.Development.json e defina Jwt:Secret (32+ caracteres aleatórios).
cd ..
dotnet run --project Api
```
A API sobe em `http://localhost:5080` com `/health` e `/swagger` (somente em desenvolvimento);
migrations aplicadas e usuários/álbum iniciais criados no primeiro start.

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
dotnet test            # 26 testes (unitários + integração)

cd ../Frontend
npm run typecheck      # checagem de tipos
npm run lint           # ESLint
npm run test           # Vitest
```

## 6. Roteiro de demonstração

1. **autor** → Autoria: edite o álbum, envie uma capa, crie figurinhas (a **tag MD5** é gerada da imagem). Anote as tags.
2. **autor** → exporte as figurinhas em **texto** e em **binário**; teste **importar binário**.
3. **colecionador** → "Adquirir figurinha": informe uma tag, veja o preview e insira; a figurinha aparece no **álbum** (duplo clique mostra os detalhes).
4. **admin** → Usuários: crie/edite/remova/filtre e **zere a senha** de um usuário.

## 7. Estrutura de diretórios

```
controle-disciplinas-ucp-novo/
├── instrucoes-figurinhas.txt   # normativo (regra superior)
├── docs/                       # entrega + referências do professor
└── src/
    ├── Backend/                # .NET 10, Clean Architecture
    │   ├── Api/                # controllers, contracts, middlewares, Program
    │   ├── Application/        # casos de uso (Auth, Usuarios, Album, Figurinhas, Colecao, Arquivos)
    │   ├── Domain/             # Usuario, Album, Figurinha, FigurinhaAdquirida, enums
    │   ├── Infrastructure/     # EF Core+SQLite, repositórios, JWT, Argon2id
    │   ├── Shared/             # kernel, constantes, helpers
    │   └── Tests/              # 26 testes
    └── Frontend/               # React 19 + Vite 8 + TS 6 + Tailwind 4 + shadcn
        └── src/                # features: auth, usuarios, album, colecao, conta
```

## 8. Checklist da rubrica (18 pts; nota final = mínimo(10, nota))

- [x] (adm) inserir/remover/editar usuários
- [x] (autor) gerenciar álbum, inserir/remover/editar figurinhas
- [x] (colecionador) visualizar e adicionar figurinha
- [x] listar e filtrar usuários
- [x] listar e filtrar figurinhas
- [x] calcular e usar a tag (hash MD5)
- [x] tela de splash e sobre
- [x] tela de login
- [x] usar banco de dados (SQLite)
- [x] arquivo texto
- [x] arquivo binário
- [x] fotos no banco (BLOB)
- [x] telas personalizadas e ícones
- [x] uso de controles personalizados (componentes reutilizáveis)
- [x] Nomes e RGUs preenchidos (seção 1)

## 9. Gerar o ZIP de entrega (sem executáveis)

No PowerShell, a partir da pasta **pai** de `controle-disciplinas-ucp-novo`:

```powershell
$origem = "controle-disciplinas-ucp-novo"
$destino = "album-figurinhas-entrega.zip"
$excluir = @("bin", "obj", "node_modules", "dist", "test-results", ".vs", ".git", "data")

$temp = Join-Path $env:TEMP "entrega-album"
Remove-Item $temp -Recurse -Force -ErrorAction SilentlyContinue
robocopy $origem $temp /E /XD $excluir /XF *.db *.db-shm *.db-wal appsettings.Development.json *.exe *.dll *.pdb | Out-Null
Compress-Archive -Path "$temp\*" -DestinationPath $destino -Force
Remove-Item $temp -Recurse -Force
Write-Host "ZIP gerado: $destino"
```

O ZIP resultante contém somente código-fonte, documentação e configurações de exemplo.
