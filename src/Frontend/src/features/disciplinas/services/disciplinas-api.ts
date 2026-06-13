import { http } from "@/core/http/client";
import type { DisciplinaDto } from "@/types/api";

export interface DisciplinaFiltros {
  nome?: string;
  professor?: string;
  ano?: number;
  semestre?: number;
}

export interface DisciplinaPayload {
  codigo: string;
  nome: string;
  professor: string | null;
  periodo: number;
  creditos: number;
}

export const disciplinasApi = {
  async listar(filtros: DisciplinaFiltros = {}): Promise<DisciplinaDto[]> {
    const { data } = await http.get<DisciplinaDto[]>("/disciplinas", { params: filtros });
    return data;
  },

  async criar(payload: DisciplinaPayload): Promise<DisciplinaDto> {
    const { data } = await http.post<DisciplinaDto>("/disciplinas", payload);
    return data;
  },

  async atualizar(id: number, payload: DisciplinaPayload): Promise<DisciplinaDto> {
    const { data } = await http.put<DisciplinaDto>(`/disciplinas/${id}`, payload);
    return data;
  },

  async remover(id: number): Promise<void> {
    await http.delete(`/disciplinas/${id}`);
  },
};
