import { z } from "zod";
import { ANO_MAX, ANO_MIN } from "@/config/constants";

export const historicoSchema = z.object({
  disciplinaId: z.coerce.number().int().min(1, "Selecione a disciplina."),
  ano: z.coerce
    .number()
    .int()
    .min(ANO_MIN, `Ano entre ${ANO_MIN} e ${ANO_MAX}.`)
    .max(ANO_MAX, `Ano entre ${ANO_MIN} e ${ANO_MAX}.`),
  semestre: z.coerce.number().int().min(1).max(2),
  periodo: z.coerce.number().int().min(1, "Período entre 1 e 20.").max(20),
  mediaFinal: z.coerce.number().min(0, "Média entre 0 e 10.").max(10, "Média entre 0 e 10."),
});

export type HistoricoFormInput = z.input<typeof historicoSchema>;
export type HistoricoFormData = z.output<typeof historicoSchema>;
