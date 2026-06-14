# ▶️ RETOMAR — onde parei e como continuar

_Atualizado em 2026-06-14. Leia este arquivo primeiro ao voltar._

## 1. Em uma frase

Projeto **migrado de "controle de disciplinas" para "ÁLBUM DE FIGURINHAS"** (o que o
professor realmente pede). **Backend e frontend completos e funcionando.** Falta só
preencher os dados dos participantes e enviar a entrega.

## 2. Onde está o trabalho (Git)

- Branch atual: **`feat/album-figurinhas`** (5 commits) — TUDO está aqui.
- A `main` está protegida (exige Pull Request). **O PR ainda NÃO foi aberto.**
- Último commit: `d06e56a` (docs README + ENTREGA).
- Para conferir: `git log --oneline -6` dentro de `controle-disciplinas-ucp-novo`.

## 3. O que JÁ está pronto ✅

- **Backend (.NET 10):** 3 perfis (Administrador/Autor/Colecionador), login por **Login**+senha,
  CRUD usuários + filtro + **zerar senha**, álbum único + capa, CRUD figurinhas + filtro + limpar,
  **tag = MD5 da imagem**, fotos no banco (BLOB), **arquivo texto e binário** (export/import),
  JWT + refresh httpOnly. **26/26 testes.**
- **Frontend (React/Vite):** login → redireciona por perfil; telas de admin, autor, colecionador,
  conta, **splash** e **sobre**. typecheck/lint/6 testes verdes; e2e validado.
- **Docs:** `instrucoes-figurinhas.txt` (normativo), `README.md`, `docs/ENTREGA.md`,
  `DECISOES.md` (D19 pivot, D20 login), `PENDENCIAS.md`.

## 4. O que FALTA fazer ⏳ (sua lista de amanhã)

1. **Preencher participantes** em `docs/ENTREGA.md` §1 (nome + RGU).
2. **(opcional) Abrir o PR** de `feat/album-figurinhas` para a `main` e mergear — peça ao Claude
   "abra o PR para a main" (ele faz via API do GitHub).
3. **Gerar o ZIP de entrega** (sem executáveis) — script pronto em `docs/ENTREGA.md` §9.
4. **Enviar a entrega:** e-mail `mozar.silva@gmail.com`, assunto **"trab ucp P3 2026 01"**,
   até **15/06/2026**, com RGU+nome, ZIP do código e login+senha.
5. **(opcional) Ajustes visuais** nas telas depois de testar.

## 5. Como subir o sistema amanhã

Os servidores de hoje serão encerrados ao fechar a sessão. Para rodar de novo:

```bash
# Terminal 1 — backend (porta 5080)
cd src/Backend
dotnet run --project Api

# Terminal 2 — frontend (porta 5173)
cd src/Frontend
npm run dev
```

Acesse **http://localhost:5173**. Se o backend reclamar de `Jwt:Secret`, confira que existe
`src/Backend/Api/appsettings.Development.json` com o segredo (não versionado).

## 6. Credenciais (senha de todos: `123456`)

| Login | Perfil | O que faz |
| :--- | :--- | :--- |
| `admin` | Administrador | gerencia usuários |
| `autor` | Autor | edita álbum e figurinhas |
| `colecionador` | Colecionador | vê o álbum e adquire figurinhas |

## 7. Roteiro rápido de teste

1. Entre como **autor** → crie figurinhas (upload de imagem gera a **tag** automática) → anote as tags.
2. Entre como **colecionador** → "Adquirir figurinha" → cole uma tag → Inserir → veja no álbum (duplo clique = detalhe).
3. Entre como **admin** → crie/edite/remova/filtre usuários e **zere a senha**.

## 8. Verificações (tudo deve passar)

```bash
cd src/Backend && dotnet test          # 26/26
cd ../Frontend && npm run typecheck && npm run lint && npm run test
```

## 9. Arquivos-chave

- **`instrucoes-figurinhas.txt`** — regra superior do projeto (adaptado do `instrucoes.txt` legado).
- **`docs/ENTREGA.md`** — execução, credenciais, rubrica e dados de entrega.
- **`docs/referencias-professor/Trabalho Algum 2026-01.*`** — enunciado do professor (pdf/rtf/txt).
- **`DECISOES.md`** / **`PENDENCIAS.md`** / **`RASTREAMENTO.md`** — histórico de decisões e ações.

> Dica: ao voltar, diga ao Claude "li o RETOMAR.md, vamos continuar" e indique o item da seção 4.
