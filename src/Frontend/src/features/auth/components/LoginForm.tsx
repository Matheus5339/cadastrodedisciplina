import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate, useSearchParams } from "react-router-dom";
import { LogIn } from "lucide-react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormField } from "@/components/forms/FormField";
import { useAuthStore } from "@/core/auth/auth-store";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { authApi } from "@/features/auth/services/auth-api";
import { loginSchema, type LoginFormData } from "@/features/auth/schemas/auth-schemas";
import { rotaInicial } from "@/routes/paths";

export function LoginForm() {
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const aplicarSessao = useAuthStore((s) => s.aplicarSessao);
  const [erro, setErro] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({ resolver: zodResolver(loginSchema) });

  async function onSubmit(dados: LoginFormData) {
    setErro(null);
    try {
      const auth = await authApi.login(dados.login, dados.senha);
      aplicarSessao(auth);
      navigate(rotaInicial(auth.usuario.perfil), { replace: true });
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
      {params.get("expirada") && !erro && (
        <Alert variant="destructive">Sua sessão expirou. Entre novamente.</Alert>
      )}
      {erro && <Alert variant="destructive">{erro}</Alert>}

      <FormField id="login" label="Login" erro={errors.login?.message}>
        <Input id="login" autoComplete="username" placeholder="seu login" {...register("login")} />
      </FormField>

      <FormField id="senha" label="Senha" erro={errors.senha?.message}>
        <Input id="senha" type="password" autoComplete="current-password" {...register("senha")} />
      </FormField>

      <Button type="submit" className="w-full" disabled={isSubmitting}>
        <LogIn className="h-4 w-4" />
        {isSubmitting ? "Entrando..." : "Entrar"}
      </Button>
    </form>
  );
}
