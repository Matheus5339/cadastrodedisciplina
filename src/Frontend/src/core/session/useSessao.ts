import { useAuthStore } from "@/core/auth/auth-store";

/** Acesso à sessão atual (aluno logado e estado de autenticação). */
export function useSessao() {
  const aluno = useAuthStore((s) => s.aluno);
  const autenticado = useAuthStore((s) => s.accessToken !== null);
  const fotoVersao = useAuthStore((s) => s.fotoVersao);
  return { aluno, autenticado, fotoVersao };
}
