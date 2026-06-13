import type { RouteObject } from "react-router-dom";
import { DisciplinasPage } from "@/features/disciplinas/pages/DisciplinasPage";
import { paths } from "@/routes/paths";

export const disciplinasRoutes: RouteObject[] = [
  { path: paths.disciplinas, element: <DisciplinasPage /> },
];
