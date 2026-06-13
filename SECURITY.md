# Política de Segurança

## Versões suportadas

Este é um projeto acadêmico; a versão suportada é sempre a da branch `main`.

## Como reportar uma vulnerabilidade

Reporte de forma privada, **sem abrir issue pública**, usando o recurso
**“Report a vulnerability”** (Security → Advisories) do repositório no GitHub,
ou por e-mail ao mantenedor. Inclua passos de reprodução e impacto. O retorno
é dado o mais breve possível.

## Postura de segurança adotada

- **Senhas:** Argon2id (MD5 e SHA-256 simples são proibidos para senhas).
- **Autenticação:** JWT Bearer de curta duração + refresh token com rotação e
  revogação de família ao detectar reuso.
- **Refresh token:** armazenado em **cookie httpOnly** (`SameSite=Lax`,
  `Secure` em HTTPS, escopo `/api/auth`) — não acessível por JavaScript.
- **Cabeçalhos HTTP:** CSP, `X-Frame-Options: DENY`, `X-Content-Type-Options:
  nosniff`, `Referrer-Policy`, `Permissions-Policy`; HSTS + redirecionamento
  HTTPS fora de desenvolvimento.
- **Rate limiting** nos endpoints de autenticação (anti brute-force).
- **Isolamento por aluno:** cada usuário acessa somente os próprios dados,
  derivados exclusivamente do token.
- **Segredos** nunca são versionados (variáveis de ambiente / user secrets).

## Nota sobre `npm audit`

As vulnerabilidades reportadas pelo `npm audit` referem-se à cadeia de
**ferramentas de desenvolvimento** (`esbuild`/`vite`/`vitest`) e afetam apenas o
servidor de desenvolvimento — **não** são embarcadas no runtime de produção.
A correção via `npm audit fix --force` exigiria substituir versões fixadas pelo
normativo do projeto, portanto não é aplicada automaticamente.
