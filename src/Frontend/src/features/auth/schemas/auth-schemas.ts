import { z } from "zod";

export const loginSchema = z.object({
  email: z.string().trim().toLowerCase().email("E-mail inválido."),
  senha: z.string().min(1, "Informe a senha."),
});

export type LoginFormData = z.infer<typeof loginSchema>;

function cpfValido(cpf: string): boolean {
  const digitos = cpf.replace(/\D/g, "");
  if (digitos.length !== 11 || new Set(digitos).size === 1) return false;
  const numeros = [...digitos].map(Number);
  for (let dv = 9; dv < 11; dv++) {
    let soma = 0;
    for (let i = 0; i < dv; i++) soma += numeros[i] * (dv + 1 - i);
    if (numeros[dv] !== ((soma * 10) % 11) % 10) return false;
  }
  return true;
}

export const registroSchema = z
  .object({
    rgu: z.string().trim().min(1, "Informe o RGU.").max(20, "RGU até 20 caracteres."),
    cpf: z.string().refine(cpfValido, "CPF inválido."),
    email: z.string().trim().toLowerCase().email("E-mail inválido."),
    nome: z.string().trim().min(3, "Nome com pelo menos 3 caracteres.").max(120),
    senha: z
      .string()
      .min(8, "Senha com pelo menos 8 caracteres.")
      .max(128)
      .regex(/[a-zA-Z]/, "Senha deve conter letra.")
      .regex(/\d/, "Senha deve conter número."),
    confirmarSenha: z.string(),
  })
  .refine((dados) => dados.senha === dados.confirmarSenha, {
    message: "As senhas não conferem.",
    path: ["confirmarSenha"],
  });

export type RegistroFormData = z.infer<typeof registroSchema>;
