import { http } from "@/core/http/client";
import type { AuthResultDto } from "@/types/api";

export const authApi = {
  async login(login: string, senha: string): Promise<AuthResultDto> {
    const { data } = await http.post<AuthResultDto>("/auth/login", { login, senha });
    return data;
  },

  async logout(): Promise<void> {
    // o refresh token vem do cookie httpOnly; nada no corpo
    await http.post("/auth/logout");
  },
};
