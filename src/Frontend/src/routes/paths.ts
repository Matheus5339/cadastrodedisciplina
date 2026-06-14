import type { Perfil } from "@/types/api";

export const paths = {
  splash: "/",
  login: "/login",
  sobre: "/sobre",
  usuarios: "/usuarios",
  autoria: "/autoria",
  album: "/album",
  novaFigurinha: "/album/nova-figurinha",
  conta: "/conta",
  naoAutorizado: "/nao-autorizado",
} as const;

/** Tela inicial de cada perfil após o login. */
export function rotaInicial(perfil: Perfil | null | undefined): string {
  switch (perfil) {
    case "Administrador":
      return paths.usuarios;
    case "Autor":
      return paths.autoria;
    case "Colecionador":
      return paths.album;
    default:
      return paths.login;
  }
}
