/** Configurações globais derivadas do ambiente. */
export const env = {
  /** Base da API. Em dev o Vite faz proxy de /api para o backend (vite.config.ts). */
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? "/api",
  appName: "Álbum de Figurinhas — UCP",
} as const;
