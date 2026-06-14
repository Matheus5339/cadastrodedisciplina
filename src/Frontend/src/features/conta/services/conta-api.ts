import { http } from "@/core/http/client";
import type { UsuarioDto } from "@/types/api";

export const contaApi = {
  obter: () => http.get<UsuarioDto>("/conta/me").then((r) => r.data),
  atualizarLogin: (login: string) => http.put<UsuarioDto>("/conta/me", { login }).then((r) => r.data),
  trocarSenha: (novaSenha: string) => http.post("/conta/me/senha", { novaSenha }),
};
