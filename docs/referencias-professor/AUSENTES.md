# Documentos do professor — análise

## Situação atualizada (2026-06-13)

O documento **`Trabalho Algum 2026-01.pdf`** foi **fornecido pelo usuário** e está
arquivado nesta pasta. (Antes estava ausente — ver histórico abaixo.)

### ⚠️ Conteúdo do documento: outro domínio

O PDF (versão 1.0.2.1, 25/05/2026) descreve um **álbum de figurinhas virtual** —
**não** um controle de disciplinas. Resumo do que o documento especifica:

- **Resumo (§1):** "Sistema para simular um album de figurinhas virtual."
- **Perfis:** Administrador (gerencia usuários), Autor (cria/edita álbum), Colecionador (visualiza/adquire figurinhas).
- **Telas:** FrmLogin, FrmUsuarios, FrmUsuario, FrmAutoria, FrmFigurinha, FrmAlbum (capa + páginas), FrmNovaFigurinha.
- **Figurinhas/álbum:** álbum único com capa e páginas; figurinhas com `tag = hash MD5 da imagem`; aquisição de figurinha por tag.

### Divergência e precedência

Isso **diverge integralmente** do `instrucoes.txt` (controle de disciplinas), que é a
**fonte normativa superior** (ordem de precedência em `docs/REGRAS-COMPLEMENTARES.md`:
1. `instrucoes.txt`; 2. README original; 3. documentos do professor; 4. implementações anteriores).

Conforme a **regra 3** das Regras Complementares — "funcionalidades específicas de
outros domínios (ex.: álbum de figurinhas) **não podem ser transportadas**" — o álbum
de figurinhas **não** foi (nem deve ser) implantado sobre o projeto de disciplinas.

> **Ação humana necessária:** o documento do professor que será **avaliado** descreve
> um sistema diferente do `instrucoes.txt`. Essa divergência precisa ser esclarecida
> com o professor / decidida pelo usuário (ver `PENDENCIAS.md` P3). O projeto atual
> segue o `instrucoes.txt`, conforme reafirmado pelo usuário.

---

## Histórico (2026-06-11)

Na primeira sessão os arquivos abaixo **não foram encontrados** (busca recursiva):

- `Trabalho Algum 2026-01.pdf` *(fornecido depois — ver acima)*
- `Trabalho Algum 2026-01.rtf.doc`
- `Trabalho Algum 2026-01.txt`

Nenhum ZIP de implementações anteriores foi encontrado. À época, nenhum requisito
específico do "álbum de figurinhas" foi lido, e os requisitos genéricos compatíveis
(banco embarcado, camadas, login, CRUD, filtros, entrega só de código) foram herdados
via `leia.txt` §3.
