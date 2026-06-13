import type { RouteObject } from "react-router-dom";
import { HistoricoPage } from "@/features/historico/pages/HistoricoPage";
import { paths } from "@/routes/paths";

export const historicoRoutes: RouteObject[] = [
  { path: paths.historico, element: <HistoricoPage /> },
];
