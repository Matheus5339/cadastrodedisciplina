import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "react-router-dom";
import { UserPlus } from "lucide-react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormField } from "@/components/forms/FormField";
import { useAuthStore } from "@/core/auth/auth-store";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { sanitizarTexto } from "@/core/security/sanitize";
import { authApi } from "@/features/auth/services/auth-api";
import { registroSchema, type RegistroFormData } from "@/features/auth/schemas/auth-schemas";
import { paths } from "@/routes/paths";

export function RegisterForm() {
  const navigate = useNavigate();
  const aplicarSessao = useAuthStore((s) => s.aplicarSessao);
  const [erro, setErro] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegistroFormData>({ resolver: zodResolver(registroSchema) });

  async function onSubmit(dados: RegistroFormData) {
    setErro(null);
    try {
      const auth = await authApi.registrar({
        rgu: sanitizarTexto(dados.rgu),
        cpf: dados.cpf.replace(/\D/g, ""),
        email: dados.email,
        nome: sanitizarTexto(dados.nome),
        senha: dados.senha,
      });
      aplicarSessao(auth);
      navigate(paths.dashboard);
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
      {erro && <Alert variant="destructive">{erro}</Alert>}

      <div className="grid gap-4 sm:grid-cols-2">
        <FormField id="rgu" label="RGU" erro={errors.rgu?.message}>
          <Input id="rgu" placeholder="2026100123" {...register("rgu")} />
        </FormField>
        <FormField id="cpf" label="CPF" erro={errors.cpf?.message}>
          <Input id="cpf" placeholder="000.000.000-00" inputMode="numeric" {...register("cpf")} />
        </FormField>
      </div>

      <FormField id="nome" label="Nome completo" erro={errors.nome?.message}>
        <Input id="nome" autoComplete="name" {...register("nome")} />
      </FormField>

      <FormField id="email" label="E-mail" erro={errors.email?.message}>
        <Input id="email" type="email" autoComplete="email" placeholder="voce@ucp.edu.br" {...register("email")} />
      </FormField>

      <div className="grid gap-4 sm:grid-cols-2">
        <FormField id="senha" label="Senha" erro={errors.senha?.message}>
          <Input id="senha" type="password" autoComplete="new-password" {...register("senha")} />
        </FormField>
        <FormField id="confirmarSenha" label="Confirmar senha" erro={errors.confirmarSenha?.message}>
          <Input id="confirmarSenha" type="password" autoComplete="new-password" {...register("confirmarSenha")} />
        </FormField>
      </div>

      <Button type="submit" className="w-full" disabled={isSubmitting}>
        <UserPlus className="h-4 w-4" />
        {isSubmitting ? "Cadastrando..." : "Criar conta"}
      </Button>
    </form>
  );
}
