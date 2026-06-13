import { http } from "@/core/http/client";
import type { AuthResultDto } from "@/types/api";

export interface RegistroPayload {
  rgu: string;
  cpf: string;
  email: string;
  nome: string;
  senha: string;
}

export const authApi = {
  async login(email: string, senha: string): Promise<AuthResultDto> {
    const { data } = await http.post<AuthResultDto>("/auth/login", { email, senha });
    return data;
  },

  async registrar(payload: RegistroPayload): Promise<AuthResultDto> {
    const { data } = await http.post<AuthResultDto>("/auth/register", payload);
    return data;
  },

  async logout(): Promise<void> {
    // o refresh token vem do cookie httpOnly; nada no corpo
    await http.post("/auth/logout");
  },
};
