import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormField } from "@/components/forms/FormField";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { disciplinasApi } from "@/features/disciplinas/services/disciplinas-api";
import {
  disciplinaSchema,
  type DisciplinaFormData,
  type DisciplinaFormInput,
} from "@/features/disciplinas/schemas/disciplina-schema";
import type { DisciplinaDto } from "@/types/api";
import { notificarSucesso } from "@/services/notification";

interface DisciplinaFormProps {
  disciplina?: DisciplinaDto | null;
  onSalva: () => void;
  onCancelar: () => void;
}

/** Formulário de criação/edição de disciplina. */
export function DisciplinaForm({ disciplina, onSalva, onCancelar }: DisciplinaFormProps) {
  const [erro, setErro] = useState<string | null>(null);
  const edicao = Boolean(disciplina);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<DisciplinaFormInput, unknown, DisciplinaFormData>({
    resolver: zodResolver(disciplinaSchema),
    defaultValues: disciplina
      ? {
          codigo: disciplina.codigo,
          nome: disciplina.nome,
          professor: disciplina.professor ?? "",
          periodo: disciplina.periodo,
          creditos: disciplina.creditos,
        }
      : { periodo: 1, creditos: 4, professor: "" },
  });

  async function onSubmit(dados: DisciplinaFormData) {
    setErro(null);
    const payload = {
      codigo: dados.codigo,
      nome: dados.nome,
      professor: dados.professor ? dados.professor : null,
      periodo: dados.periodo,
      creditos: dados.creditos,
    };
    try {
      if (disciplina) {
        await disciplinasApi.atualizar(disciplina.id, payload);
        notificarSucesso("Disciplina atualizada.");
      } else {
        await disciplinasApi.criar(payload);
        notificarSucesso("Disciplina cadastrada.");
      }
      onSalva();
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
      {erro && <Alert variant="destructive">{erro}</Alert>}

      <div className="grid gap-4 sm:grid-cols-2">
        <FormField id="codigo" label="Código" erro={errors.codigo?.message}>
          <Input id="codigo" placeholder="ECOMP-001" {...register("codigo")} />
        </FormField>
        <FormField id="professor" label="Professor (opcional)" erro={errors.professor?.message}>
          <Input id="professor" {...register("professor")} />
        </FormField>
      </div>

      <FormField id="nome" label="Nome da disciplina" erro={errors.nome?.message}>
        <Input id="nome" {...register("nome")} />
      </FormField>

      <div className="grid gap-4 sm:grid-cols-2">
        <FormField id="periodo" label="Período" erro={errors.periodo?.message}>
          <Input id="periodo" type="number" min={1} max={20} {...register("periodo")} />
        </FormField>
        <FormField id="creditos" label="Créditos" erro={errors.creditos?.message}>
          <Input id="creditos" type="number" min={0} max={30} {...register("creditos")} />
        </FormField>
      </div>

      <div className="flex justify-end gap-2 pt-2">
        <Button variant="outline" onClick={onCancelar} disabled={isSubmitting}>
          Cancelar
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Salvando..." : edicao ? "Salvar alterações" : "Cadastrar"}
        </Button>
      </div>
    </form>
  );
}
