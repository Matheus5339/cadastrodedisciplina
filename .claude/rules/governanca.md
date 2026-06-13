# Regras de Governança — Claude Code

Regras de execução obrigatórias para qualquer sessão de agente neste projeto.

1. **Consultar `instrucoes.txt` antes de cada etapa.** Validar compatibilidade da ação antes de criar, alterar, remover ou executar qualquer item.
2. **Respeitar a arquitetura física dos diretórios** definida em `instrucoes.txt` §5 (frontend) e §6 (backend): `src/Backend/{Api,Application,Domain,Infrastructure,Shared,Tests}` e a árvore do frontend.
3. **Não achatar as camadas do backend.** Api, Application, Domain, Infrastructure, Shared e Tests são projetos/pastas distintos com responsabilidades próprias.
4. **Não colocar regra de negócio em controllers.** Controllers apenas recebem requests, delegam para Application e devolvem responses.
5. **Não acoplar Domain a banco, API ou frontend.** O Domain não referencia EF Core, HTTP nem bibliotecas de infraestrutura.
6. **Não gravar segredos no repositório.** Chaves JWT, peppers, senhas e tokens vêm de variáveis de ambiente, user secrets ou arquivos locais ignorados pelo Git.
7. **Não alegar sucesso sem executar validações reais.** Build e testes devem ser executados de verdade; o resultado real (incluindo falhas) é registrado em `RASTREAMENTO.md`.
8. **Não copiar código das versões anteriores.** A implementação antiga em `..\src\` serve apenas como referência de aprendizado.
9. **Não resolver divergências silenciosamente.** Conflitos entre documentos seguem a ordem de precedência; o que ela não resolver vai para `PENDENCIAS.md` e aguarda decisão humana.
10. **Registrar decisões e resultados.** Decisões em `DECISOES.md`; ações e resultados em `RASTREAMENTO.md`; tarefas em `TAREFAS.md`; ambiguidades em `PENDENCIAS.md`.
