import { useAuthStore } from "@/core/auth/auth-store";

/** Acesso à sessão atual (usuário logado, perfil e estado de autenticação). */
export function useSessao() {
  const usuario = useAuthStore((s) => s.usuario);
  const autenticado = useAuthStore((s) => s.accessToken !== null);
  return { usuario, perfil: usuario?.perfil ?? null, autenticado };
}
