import { createBrowserRouter } from "react-router-dom";
import { AppLayout } from "@/components/layout/AppLayout";
import { LoginPage } from "@/features/auth/pages/LoginPage";
import { UsuariosPage } from "@/features/usuarios/pages/UsuariosPage";
import { AutoriaPage } from "@/features/album/pages/AutoriaPage";
import { AlbumPage } from "@/features/colecao/pages/AlbumPage";
import { NovaFigurinhaPage } from "@/features/colecao/pages/NovaFigurinhaPage";
import { ContaPage } from "@/features/conta/pages/ContaPage";
import { SplashPage } from "@/pages/SplashPage";
import { SobrePage } from "@/pages/SobrePage";
import { ErrorPage } from "@/pages/ErrorPage";
import { NotFoundPage } from "@/pages/NotFoundPage";
import { UnauthorizedPage } from "@/pages/UnauthorizedPage";
import { ProtectedRoute, PublicOnlyRoute, RoleRoute } from "@/routes/ProtectedRoute";
import { paths } from "@/routes/paths";

export const router = createBrowserRouter([
  {
    errorElement: <ErrorPage />,
    children: [
      { path: paths.splash, element: <SplashPage /> },
      { path: paths.sobre, element: <SobrePage /> },
      {
        element: <PublicOnlyRoute />,
        children: [{ path: paths.login, element: <LoginPage /> }],
      },
      {
        element: <ProtectedRoute />,
        children: [
          {
            element: <AppLayout />,
            children: [
              {
                element: <RoleRoute perfil="Administrador" />,
                children: [{ path: paths.usuarios, element: <UsuariosPage /> }],
              },
              {
                element: <RoleRoute perfil="Autor" />,
                children: [{ path: paths.autoria, element: <AutoriaPage /> }],
              },
              {
                element: <RoleRoute perfil="Colecionador" />,
                children: [
                  { path: paths.album, element: <AlbumPage /> },
                  { path: paths.novaFigurinha, element: <NovaFigurinhaPage /> },
                ],
              },
              { path: paths.conta, element: <ContaPage /> },
            ],
          },
        ],
      },
      { path: paths.naoAutorizado, element: <UnauthorizedPage /> },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);
