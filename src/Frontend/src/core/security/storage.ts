/**
 * Armazenamento seguro da sessão (segurança 14).
 * Usa sessionStorage: o token não sobrevive ao fechamento da aba/navegador,
 * reduzindo a janela de exposição em máquinas compartilhadas.
 */
const PREFIXO = "cdu:";

export const secureStorage = {
  get<T>(chave: string): T | null {
    try {
      const bruto = sessionStorage.getItem(PREFIXO + chave);
      return bruto ? (JSON.parse(bruto) as T) : null;
    } catch {
      return null;
    }
  },
  set(chave: string, valor: unknown): void {
    sessionStorage.setItem(PREFIXO + chave, JSON.stringify(valor));
  },
  remove(chave: string): void {
    sessionStorage.removeItem(PREFIXO + chave);
  },
  clear(): void {
    for (const k of Object.keys(sessionStorage)) {
      if (k.startsWith(PREFIXO)) sessionStorage.removeItem(k);
    }
  },
};
