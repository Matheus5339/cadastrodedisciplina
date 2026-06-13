/** Formata o CR com 2 casas, no padrão brasileiro. */
export function formatarCr(cr: number | null | undefined): string {
  if (cr === null || cr === undefined) return "—";
  return cr.toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

export function formatarMedia(media: number): string {
  return media.toLocaleString("pt-BR", { minimumFractionDigits: 1, maximumFractionDigits: 2 });
}

/** Aplica máscara visual de CPF (000.000.000-00). */
export function formatarCpf(cpf: string): string {
  const d = cpf.replace(/\D/g, "").slice(0, 11);
  return d
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
}
