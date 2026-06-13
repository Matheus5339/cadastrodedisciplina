import { useCallback, useEffect, useState } from "react";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import {
  disciplinasApi,
  type DisciplinaFiltros,
} from "@/features/disciplinas/services/disciplinas-api";
import type { DisciplinaDto } from "@/types/api";

/** Carrega a lista de disciplinas reagindo aos filtros. */
export function useDisciplinas(filtros: DisciplinaFiltros) {
  const [disciplinas, setDisciplinas] = useState<DisciplinaDto[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const recarregar = useCallback(async () => {
    setCarregando(true);
    setErro(null);
    try {
      setDisciplinas(await disciplinasApi.listar(filtros));
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    } finally {
      setCarregando(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtros.nome, filtros.professor, filtros.ano, filtros.semestre]);

  useEffect(() => {
    void recarregar();
  }, [recarregar]);

  return { disciplinas, carregando, erro, recarregar };
}
