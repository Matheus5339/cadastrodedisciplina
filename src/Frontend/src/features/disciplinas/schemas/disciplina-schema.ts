import { z } from "zod";

export const disciplinaSchema = z.object({
  codigo: z.string().trim().min(1, "Informe o código.").max(20, "Código até 20 caracteres."),
  nome: z.string().trim().min(1, "Informe o nome.").max(150),
  professor: z.string().trim().max(120).optional().or(z.literal("")),
  periodo: z.coerce.number().int().min(1, "Período entre 1 e 20.").max(20, "Período entre 1 e 20."),
  creditos: z.coerce.number().int().min(0, "Créditos entre 0 e 30.").max(30, "Créditos entre 0 e 30."),
});

export type DisciplinaFormInput = z.input<typeof disciplinaSchema>;
export type DisciplinaFormData = z.output<typeof disciplinaSchema>;
