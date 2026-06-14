import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useSessao } from "@/core/session/useSessao";
import { paths, rotaInicial } from "@/routes/paths";
import type { Perfil } from "@/types/api";

/** Rota protegida: redireciona não autenticados para o login. */
export function ProtectedRoute() {
  const { autenticado } = useSessao();
  const location = useLocation();

  if (!autenticado) {
    return <Navigate to={paths.login} replace state={{ de: location.pathname }} />;
  }
  return <Outlet />;
}

/** Rota pública exclusiva (login): autenticado vai para a tela inicial do seu perfil. */
export function PublicOnlyRoute() {
  const { autenticado, perfil } = useSessao();
  if (autenticado) {
    return <Navigate to={rotaInicial(perfil)} replace />;
  }
  return <Outlet />;
}

/** Restringe a rota a um perfil específico. */
export function RoleRoute({ perfil }: { perfil: Perfil }) {
  const { perfil: atual } = useSessao();
  if (atual !== perfil) {
    return <Navigate to={paths.naoAutorizado} replace />;
  }
  return <Outlet />;
}
