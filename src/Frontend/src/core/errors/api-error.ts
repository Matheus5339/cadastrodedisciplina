import { AxiosError } from "axios";
import type { ApiErrorBody } from "@/types/api";

/** Extrai mensagem amigável do formato de erro padronizado da API. */
export function obterMensagemDeErro(erro: unknown): string {
  if (erro instanceof AxiosError) {
    const corpo = erro.response?.data as ApiErrorBody | undefined;
    if (corpo?.detalhe) return corpo.detalhe;
    if (erro.response?.status === 401) return "Sessão expirada ou credenciais inválidas.";
    if (erro.code === "ERR_NETWORK") return "Não foi possível conectar ao servidor.";
  }
  return "Ocorreu um erro inesperado. Tente novamente.";
}
