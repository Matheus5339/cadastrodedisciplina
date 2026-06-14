import { create } from "zustand";
import { persist, createJSONStorage } from "zustand/middleware";
import type { AuthResultDto, UsuarioDto } from "@/types/api";

interface AuthState {
  accessToken: string | null;
  usuario: UsuarioDto | null;
  aplicarSessao: (auth: AuthResultDto) => void;
  atualizarUsuario: (usuario: UsuarioDto) => void;
  limparSessao: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      usuario: null,
      // o refresh token fica no cookie httpOnly; aqui só guardamos o access token
      aplicarSessao: (auth) => set({ accessToken: auth.accessToken, usuario: auth.usuario }),
      atualizarUsuario: (usuario) => set({ usuario }),
      limparSessao: () => set({ accessToken: null, usuario: null }),
    }),
    {
      name: "cdu:auth",
      storage: createJSONStorage(() => sessionStorage),
    },
  ),
);

export const estaAutenticado = () => useAuthStore.getState().accessToken !== null;
