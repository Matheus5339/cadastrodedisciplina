import type { ReactNode } from "react";
import { useSessao } from "@/core/session/useSessao";

interface PermissionGateProps {
  children: ReactNode;
  fallback?: ReactNode;
}

/**
 * Autorização visual: só renderiza o conteúdo para usuário autenticado.
 * O sistema tem um único papel (aluno); o gate existe como ponto único de
 * extensão caso novos papéis surjam.
 */
export function PermissionGate({ children, fallback = null }: PermissionGateProps) {
  const { autenticado } = useSessao();
  return autenticado ? <>{children}</> : <>{fallback}</>;
}
