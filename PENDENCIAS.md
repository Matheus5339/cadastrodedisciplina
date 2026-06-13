# Pendências

| ID | Ambiguidade / Impedimento | Alternativas | Risco | Decisão humana necessária |
| :--- | :--- | :--- | :--- | :--- |
| P1 | ~~Nenhum SDK .NET instalado~~ — **RESOLVIDA em 2026-06-12**: usuário autorizou; SDK **.NET 10.0.301** instalado via `dotnet-install.ps1` oficial em `%LOCALAPPDATA%\Microsoft\dotnet` (escopo de usuário, sem admin); `PATH` e `DOTNET_ROOT` do usuário atualizados; `dotnet --list-sdks` confirma 10.0.301 | — | — | Não (resolvida) |
| P2 | ~~Documentos do professor ausentes~~ — **RESOLVIDA em 2026-06-12**: usuário escolheu (a) prosseguir sem eles (resposta "2a"). Se fornecidos depois, colocar em `docs/referencias-professor/` e atualizar a análise | — | — | Não (resolvida) |
| P3 | **Nenhum arquivo ZIP** de implementações anteriores existe (busca recursiva executada); a única referência anterior é `../src` | Prosseguir usando `../src` como referência de aprendizado | Baixo — a referência viva existe e foi inspecionada | Não (informativo) |
| P4 | ~~Fórmula do CR (hipótese)~~ — **RESOLVIDA em 2026-06-12**: usuário confirmou a média ponderada por créditos (resposta "3a"); D4 consolidada | — | — | Não (resolvida) |
| P5 | ~~Disponibilidade das versões npm~~ — **RESOLVIDA em 2026-06-12**: `npm install` resolveu todas as versões exatas do normativo (react 19.2.6, vite 8.0.14, typescript 6.0.2, tailwindcss 4.3.0, shadcn 3.2.1). Nenhum fallback foi necessário | — | — | Não (resolvida) |
| P6 | ~~Ativação do hook~~ — **RESOLVIDA em 2026-06-12**: usuário escolheu (a) abrir as próximas sessões a partir de `controle-disciplinas-ucp-novo` (resposta "4a"). Na sessão atual as regras são cumpridas por disciplina do agente | — | — | Não (resolvida) |
| P7 | E-mail/assunto da entrega e dados dos participantes (nome, RGU) desconhecidos | Usuário preenche `docs/ENTREGA.md` na Fase 4 | Entrega incompleta | **SIM** — fornecer na Fase 4 |
