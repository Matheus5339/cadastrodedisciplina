import { useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Camera } from "lucide-react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { FormField } from "@/components/forms/FormField";
import { UserAvatar } from "@/components/layout/UserAvatar";
import { useAuthStore } from "@/core/auth/auth-store";
import { useSessao } from "@/core/session/useSessao";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { alunoApi } from "@/features/profile/services/aluno-api";
import { perfilSchema, type PerfilFormData } from "@/features/profile/schemas/perfil-schema";
import { FOTO_MAX_BYTES, FOTO_TIPOS_PERMITIDOS } from "@/config/constants";
import { formatarCpf } from "@/lib/formatters";
import { notificarErro, notificarSucesso } from "@/services/notification";

export function PerfilPage() {
  const { aluno } = useSessao();
  const { atualizarAluno, marcarFotoAlterada } = useAuthStore();
  const [erro, setErro] = useState<string | null>(null);
  const [enviandoFoto, setEnviandoFoto] = useState(false);
  const inputFotoRef = useRef<HTMLInputElement>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<PerfilFormData>({
    resolver: zodResolver(perfilSchema),
    defaultValues: { nome: aluno?.nome ?? "", email: aluno?.email ?? "" },
  });

  async function onSubmit(dados: PerfilFormData) {
    setErro(null);
    try {
      const atualizado = await alunoApi.atualizarMeusDados(dados.nome, dados.email);
      atualizarAluno(atualizado);
      notificarSucesso("Dados atualizados.");
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    }
  }

  async function aoEscolherFoto(evento: React.ChangeEvent<HTMLInputElement>) {
    const arquivo = evento.target.files?.[0];
    evento.target.value = ""; // permite reenviar o mesmo arquivo
    if (!arquivo) return;

    if (!FOTO_TIPOS_PERMITIDOS.includes(arquivo.type)) {
      notificarErro("Tipo de foto não permitido. Use JPEG, PNG ou WebP.");
      return;
    }
    if (arquivo.size > FOTO_MAX_BYTES) {
      notificarErro("A foto deve ter no máximo 2 MB.");
      return;
    }

    setEnviandoFoto(true);
    try {
      await alunoApi.enviarFoto(arquivo);
      const atualizado = await alunoApi.obterMeusDados();
      atualizarAluno(atualizado);
      marcarFotoAlterada();
      notificarSucesso("Foto atualizada.");
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    } finally {
      setEnviandoFoto(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Meu perfil</h1>
        <p className="text-sm text-muted-foreground">Seus dados de aluno e foto.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Foto</CardTitle>
          <CardDescription>JPEG, PNG ou WebP, até 2 MB.</CardDescription>
        </CardHeader>
        <CardContent className="flex items-center gap-4">
          <UserAvatar className="h-20 w-20 text-xl" />
          <div>
            <input
              ref={inputFotoRef}
              type="file"
              accept={FOTO_TIPOS_PERMITIDOS.join(",")}
              className="hidden"
              onChange={aoEscolherFoto}
            />
            <Button variant="outline" onClick={() => inputFotoRef.current?.click()} disabled={enviandoFoto}>
              <Camera className="h-4 w-4" />
              {enviandoFoto ? "Enviando..." : "Alterar foto"}
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Dados pessoais</CardTitle>
          <CardDescription>RGU e CPF não podem ser alterados.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
            {erro && <Alert variant="destructive">{erro}</Alert>}

            <div className="grid gap-4 sm:grid-cols-2">
              <FormField id="rgu" label="RGU">
                <Input id="rgu" value={aluno?.rgu ?? ""} disabled />
              </FormField>
              <FormField id="cpf" label="CPF">
                <Input id="cpf" value={formatarCpf(aluno?.cpf ?? "")} disabled />
              </FormField>
            </div>

            <FormField id="nome" label="Nome completo" erro={errors.nome?.message}>
              <Input id="nome" {...register("nome")} />
            </FormField>

            <FormField id="email" label="E-mail" erro={errors.email?.message}>
              <Input id="email" type="email" {...register("email")} />
            </FormField>

            <div className="flex justify-end">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Salvando..." : "Salvar"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
