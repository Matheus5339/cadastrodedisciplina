import type { RouteObject } from "react-router-dom";
import { LoginPage } from "@/features/auth/pages/LoginPage";
import { RegisterPage } from "@/features/auth/pages/RegisterPage";
import { paths } from "@/routes/paths";

export const authRoutes: RouteObject[] = [
  { path: paths.login, element: <LoginPage /> },
  { path: paths.cadastro, element: <RegisterPage /> },
];
