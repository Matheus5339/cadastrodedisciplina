import { create } from "zustand";
import { persist, createJSONStorage } from "zustand/middleware";
import type { AlunoDto, AuthResultDto } from "@/types/api";

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  aluno: AlunoDto | null;
  /** incrementado quando a foto muda, para forçar recarregamento do avatar */
  fotoVersao: number;
  aplicarSessao: (auth: AuthResultDto) => void;
  atualizarAluno: (aluno: AlunoDto) => void;
  marcarFotoAlterada: () => void;
  limparSessao: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      aluno: null,
      fotoVersao: 0,
      aplicarSessao: (auth) =>
        set({ accessToken: auth.accessToken, refreshToken: auth.refreshToken, aluno: auth.aluno }),
      atualizarAluno: (aluno) => set({ aluno }),
      marcarFotoAlterada: () => set((s) => ({ fotoVersao: s.fotoVersao + 1 })),
      limparSessao: () =>
        set({ accessToken: null, refreshToken: null, aluno: null, fotoVersao: 0 }),
    }),
    {
      name: "cdu:auth",
      // sessionStorage: sessão não persiste após fechar o navegador (segurança 14)
      storage: createJSONStorage(() => sessionStorage),
    },
  ),
);

export const estaAutenticado = () => useAuthStore.getState().accessToken !== null;
