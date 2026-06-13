import { http } from "@/core/http/client";
import type { AlunoDto } from "@/types/api";

export const alunoApi = {
  async obterMeusDados(): Promise<AlunoDto> {
    const { data } = await http.get<AlunoDto>("/alunos/me");
    return data;
  },

  async atualizarMeusDados(nome: string, email: string): Promise<AlunoDto> {
    const { data } = await http.put<AlunoDto>("/alunos/me", { nome, email });
    return data;
  },

  async enviarFoto(arquivo: File): Promise<void> {
    const form = new FormData();
    form.append("foto", arquivo);
    await http.put("/alunos/me/foto", form);
  },

  async obterFotoBlob(): Promise<Blob> {
    const { data } = await http.get("/alunos/me/foto", { responseType: "blob" });
    return data as Blob;
  },
};
