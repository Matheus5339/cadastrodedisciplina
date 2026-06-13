import { z } from "zod";

export const perfilSchema = z.object({
  nome: z.string().trim().min(3, "Nome com pelo menos 3 caracteres.").max(120),
  email: z.string().trim().toLowerCase().email("E-mail inválido."),
});

export type PerfilFormData = z.infer<typeof perfilSchema>;
