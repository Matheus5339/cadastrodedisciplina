import { createBrowserRouter } from "react-router-dom";
import { AppLayout } from "@/components/layout/AppLayout";
import { authRoutes } from "@/features/auth/routes";
import { disciplinasRoutes } from "@/features/disciplinas/routes";
import { historicoRoutes } from "@/features/historico/routes";
import { DashboardPage } from "@/features/dashboard/pages/DashboardPage";
import { PerfilPage } from "@/features/profile/pages/PerfilPage";
import { ErrorPage } from "@/pages/ErrorPage";
import { NotFoundPage } from "@/pages/NotFoundPage";
import { UnauthorizedPage } from "@/pages/UnauthorizedPage";
import { ProtectedRoute, PublicOnlyRoute } from "@/routes/ProtectedRoute";
import { paths } from "@/routes/paths";

export const router = createBrowserRouter([
  {
    errorElement: <ErrorPage />,
    children: [
      {
        element: <PublicOnlyRoute />,
        children: authRoutes,
      },
      {
        element: <ProtectedRoute />,
        children: [
          {
            element: <AppLayout />,
            children: [
              { path: paths.dashboard, element: <DashboardPage /> },
              ...disciplinasRoutes,
              ...historicoRoutes,
              { path: paths.perfil, element: <PerfilPage /> },
            ],
          },
        ],
      },
      { path: paths.naoAutorizado, element: <UnauthorizedPage /> },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);
