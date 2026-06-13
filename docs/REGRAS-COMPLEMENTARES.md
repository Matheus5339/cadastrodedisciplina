# Regras Complementares de Governança

Estas regras complementam `instrucoes.txt` e valem para toda e qualquer ação neste projeto.

1. **`instrucoes.txt` é a fonte normativa superior.** Define objetivo, requisitos, modelo de dados, tecnologias, estrutura física do frontend, estrutura física do backend e persistência SQLite. Não pode ser modificado nem substituído silenciosamente.
2. **O README original (`docs/referencias/README-ORIGINAL.md`) é fonte técnica complementar.** Somente regras compatíveis com `instrucoes.txt` podem ser aproveitadas — especialmente segurança, autenticação, banco, configurações sensíveis, CORS, tratamento de erros e health checks.
3. **Documentos do professor são referências acadêmicas complementares.** Apenas requisitos genéricos compatíveis podem ser aproveitados. Funcionalidades específicas de outros domínios (ex.: álbum de figurinhas) não podem ser transportadas. *(Nota: os documentos não foram encontrados no diretório de referência — ver `PENDENCIAS.md`.)*
4. **Implementações anteriores não podem ser copiadas.** Nem código, nem bancos de dados, nem builds, nem configurações inseguras. Servem apenas como referência de aprendizado.
5. **Toda ação deve respeitar a arquitetura obrigatória** definida em `instrucoes.txt` (frontend §5, backend §6) e no plano de camadas `src/Backend/{Api,Application,Domain,Infrastructure,Shared,Tests}`.
6. **Backend e frontend devem ser implementados integralmente.** Não há entregas parciais silenciosas.
7. **Segurança não é opcional.** Argon2id para senhas; MD5 e SHA-256 simples são proibidos para senhas. JWT seguro, refresh token com rotação, CORS restritivo, validação de entrada, isolamento por aluno.
8. **Resultados de build e testes nunca podem ser inventados.** Só registrar resultados realmente executados, com saída real.
9. **Suposições devem ser registradas** em `DECISOES.md` antes de serem consolidadas.
10. **Divergências entre documentos devem ser registradas** em `DECISOES.md` ou `PENDENCIAS.md`, nunca resolvidas silenciosamente.
11. **Nenhuma substituição tecnológica pode ocorrer silenciosamente.** Mudanças de versão ou de biblioteca exigem registro e, quando relevantes, autorização humana.
12. **Nenhuma escrita pode ocorrer fora da pasta `controle-disciplinas-ucp-novo`.** Arquivos externos de referência não podem ser alterados nem removidos. *(Exceção documentada: o `CLAUDE.md` do diretório pai exige registro de interações em `rastreamento.md` na raiz externa — apenas esse append é permitido.)*
13. **Segredos nunca podem ser gravados em arquivos versionados.** Usar variáveis de ambiente, user secrets ou configuração local não versionada.
14. **Ordem de precedência obrigatória:** 1) `instrucoes.txt`; 2) README original; 3) documentos do professor; 4) implementações anteriores (somente aprendizado).
