import { http } from "@/core/http/client";
import type { Perfil, UsuarioDto } from "@/types/api";

export const usuariosApi = {
  listar: (filtro?: string) =>
    http.get<UsuarioDto[]>("/usuarios", { params: filtro ? { filtro } : undefined }).then((r) => r.data),
  criar: (login: string, senha: string, perfil: Perfil) =>
    http.post<UsuarioDto>("/usuarios", { login, senha, perfil }).then((r) => r.data),
  atualizar: (id: number, login: string, perfil: Perfil) =>
    http.put<UsuarioDto>(`/usuarios/${id}`, { login, perfil }).then((r) => r.data),
  remover: (id: number) => http.delete(`/usuarios/${id}`),
  resetarSenha: (id: number) =>
    http.post<{ senha: string }>(`/usuarios/${id}/resetar-senha`).then((r) => r.data),
};
