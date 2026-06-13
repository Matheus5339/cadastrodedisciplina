import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { env } from "@/config/env";
import { useAuthStore } from "@/core/auth/auth-store";
import type { AuthResultDto } from "@/types/api";

/**
 * Cliente HTTP com interceptadores (segurança 15):
 * - anexa o Bearer token a cada requisição;
 * - em 401, tenta UMA renovação via refresh token (single-flight) e repete a
 *   requisição original; se falhar, encerra a sessão (renovação de sessão).
 */
export const http = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: 30_000,
});

http.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

let renovacaoEmAndamento: Promise<string | null> | null = null;

async function renovarSessao(): Promise<string | null> {
  const { refreshToken, aplicarSessao, limparSessao } = useAuthStore.getState();
  if (!refreshToken) return null;
  try {
    // axios "cru" para não entrar nos interceptadores e causar loop
    const { data } = await axios.post<AuthResultDto>(`${env.apiBaseUrl}/auth/refresh`, {
      refreshToken,
    });
    aplicarSessao(data);
    return data.accessToken;
  } catch {
    limparSessao();
    return null;
  }
}

http.interceptors.response.use(
  (resposta) => resposta,
  async (erro: AxiosError) => {
    const original = erro.config as (InternalAxiosRequestConfig & { _retentada?: boolean }) | undefined;
    const ehRotaDeAuth = original?.url?.includes("/auth/");

    if (erro.response?.status === 401 && original && !original._retentada && !ehRotaDeAuth) {
      original._retentada = true;
      renovacaoEmAndamento ??= renovarSessao().finally(() => {
        renovacaoEmAndamento = null;
      });
      const novoToken = await renovacaoEmAndamento;
      if (novoToken) {
        original.headers.Authorization = `Bearer ${novoToken}`;
        return http(original);
      }
      // sessão expirada de vez: manda para o login
      if (window.location.pathname !== "/login") {
        window.location.assign("/login?expirada=1");
      }
    }
    return Promise.reject(erro);
  },
);
