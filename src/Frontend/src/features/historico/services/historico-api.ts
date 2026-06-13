import { http } from "@/core/http/client";
import type { CrDto, HistoricoDto } from "@/types/api";

export interface HistoricoFiltros {
  ano?: number;
  semestre?: number;
  disciplina?: string;
  professor?: string;
}

export interface HistoricoPayload {
  disciplinaId: number;
  ano: number;
  semestre: number;
  periodo: number;
  mediaFinal: number;
}

export const historicoApi = {
  async listar(filtros: HistoricoFiltros = {}): Promise<HistoricoDto[]> {
    const { data } = await http.get<HistoricoDto[]>("/historicos", { params: filtros });
    return data;
  },

  async obterCr(): Promise<CrDto> {
    const { data } = await http.get<CrDto>("/historicos/cr");
    return data;
  },

  async criar(payload: HistoricoPayload): Promise<HistoricoDto> {
    const { data } = await http.post<HistoricoDto>("/historicos", payload);
    return data;
  },

  async atualizar(id: number, payload: HistoricoPayload): Promise<HistoricoDto> {
    const { data } = await http.put<HistoricoDto>(`/historicos/${id}`, payload);
    return data;
  },

  async remover(id: number): Promise<void> {
    await http.delete(`/historicos/${id}`);
  },
};
