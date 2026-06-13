import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useSessao } from "@/core/session/useSessao";
import { paths } from "@/routes/paths";

/** Rota protegida: redireciona não autenticados para o login. */
export function ProtectedRoute() {
  const { autenticado } = useSessao();
  const location = useLocation();

  if (!autenticado) {
    return <Navigate to={paths.login} replace state={{ de: location.pathname }} />;
  }
  return <Outlet />;
}

/** Rota pública exclusiva (login/cadastro): autenticado vai para o painel. */
export function PublicOnlyRoute() {
  const { autenticado } = useSessao();
  if (autenticado) {
    return <Navigate to={paths.dashboard} replace />;
  }
  return <Outlet />;
}
