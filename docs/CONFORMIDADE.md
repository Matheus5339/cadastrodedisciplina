# Conformidade e Validação — Álbum de Figurinhas (UCP)

Documento de evidências para a entrega. Reúne dois relatórios:
1. **Conformidade com os requisitos técnicos** exigidos.
2. **Validação prática da rubrica** do professor (18 pontos).

> Todas as verificações abaixo foram **executadas de verdade** contra a aplicação em
> funcionamento (API em `http://localhost:5080`) e/ou conferidas no código-fonte.

---

## 1. Requisitos técnicos da entrega

| Requisito | Exigido | No projeto | Evidência | OK |
| :--- | :--- | :--- | :--- | :--: |
| Tipo de aplicação | desktop **ou** web | **Web** (API REST + SPA) | `src/Backend` (API) + `src/Frontend` (SPA) | ✅ |
| Backend em C# | C# (.NET Core 2–10 ou .NET Framework 1–5) | **C# em .NET 10** | `<TargetFramework>net10.0</TargetFramework>` em todos os `.csproj` | ✅ |
| Dividido em camadas | sim | **6 projetos** (Clean Architecture) | `Api`, `Application`, `Domain`, `Infrastructure`, `Shared`, `Tests` | ✅ |
| Frontend | HTML/TS/CSS **ou** C# (WinForms/WPF/MAUI) | **TypeScript + CSS** (React/Vite) | 67 arquivos `.ts/.tsx`, `tsconfig.json`, CSS global | ✅ |
| Persistência | SQLite | **SQLite via EF Core** | `Microsoft.EntityFrameworkCore.Sqlite`; `opt.UseSqlite(...)`; migrations em runtime | ✅ |

### Detalhes que comprovam as camadas
- O **Domain** não referencia EF Core, HTTP nem bibliotecas de infraestrutura.
- Regras de negócio ficam em **Application** (Features/casos de uso), não nos controllers.
- **Infrastructure** concentra SQLite (EF Core), JWT e Argon2id.
- **Api** apenas expõe os endpoints REST e delega para Application.
- Banco SQLite criado por **migrations em runtime** (nunca versionado).

> **Observação sobre o frontend:** a regra aceita "HTML/TS/CSS" **ou** um frontend em C#.
> O projeto usa **TypeScript/CSS**, que se enquadra na primeira opção. Caso o professor
> exija especificamente frontend em C#, este é o único ponto a confirmar — pela regra
> escrita, TS/CSS está dentro do permitido.

---

## 2. Validação prática da rubrica (18 pontos)

Nota final = **mínimo(10, pontuação)**. O projeto cobre os 14 critérios (18 pts), com
**margem de folga**. Cada item abaixo foi testado via API.

| Item da rubrica | Pts | Evidência prática | OK |
| :--- | :--: | :--- | :--: |
| (adm) inserir/remover/editar usuários | 2 | criar → 201, editar → 200, remover → 204 | ✅ |
| (autor) gerenciar álbum + figurinhas | 2 | álbum PUT → 200; figurinha criar → 201, editar → 200, remover → 204 | ✅ |
| (colecionador) visualizar e adicionar | 2 | consultar por tag → 200, adquirir → 201, aparece adquirida no álbum | ✅ |
| listar e filtrar usuários | 1 | filtro por parte do login retorna só o alvo | ✅ |
| listar e filtrar figurinhas | 1 | `?texto=Messi` → resultado correto | ✅ |
| calcular e usar a tag (hash MD5) | 1 | tag do servidor == `md5(imagem)` calculado localmente (idêntico) | ✅ |
| tela de splash e sobre | 1 | `pages/SplashPage.tsx` e `pages/SobrePage.tsx` presentes | ✅ |
| tela de login | 1 | 3 perfis logam e são identificados (Administrador/Autor/Colecionador) | ✅ |
| usar banco de dados (SQLite) | 2 | figurinhas persistidas e lidas do banco | ✅ |
| arquivo texto | 1 | exportação `text/plain` com cabeçalho + linhas das figurinhas | ✅ |
| arquivo binário | 1 | exportação com *magic* `FIGB` + reimportação (dedup por tag) | ✅ |
| fotos no banco (BLOB) | 1 | `/api/figurinhas/{id}/imagem` devolve a imagem (PNG/JPEG) do banco | ✅ |
| telas personalizadas e ícones | 1 | páginas próprias + identidade visual | ✅ |
| controles personalizados (user control) | 1 | `components/ui` (shadcn) e `components/feedback` (dialog/empty/loading) | ✅ |

**Extras validados (boas práticas exigidas pelo enunciado):**
- Controle de acesso por perfil: colecionador → `/api/usuarios` retorna **403**; admin não cria figurinha (**403**).
- Administrador **zera a senha** de um usuário (devolve a senha padrão).
- Qualquer usuário **troca o próprio login e senha** (mas não o perfil): PUT login → 200, troca senha → 204, novo login válido.

**Resultado da bateria automatizada:** `26/26 verificações OK` (nenhuma falha), sem deixar
dados de teste no banco.

---

## 3. Como reproduzir as validações

```bash
# subir a aplicação
cd src/Backend && dotnet run --project Api      # API em http://localhost:5080
cd src/Frontend && npm install && npm run dev   # SPA em http://localhost:5173

# testes automatizados do backend
cd src/Backend && dotnet test                   # 26/26
```

Credenciais de demonstração (senha de todos: `123456`): `admin` (Administrador),
`autor` (Autor), `colecionador` (Colecionador).
