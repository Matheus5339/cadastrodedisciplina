import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { FormField } from "@/components/forms/FormField";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { disciplinasApi } from "@/features/disciplinas/services/disciplinas-api";
import { historicoApi } from "@/features/historico/services/historico-api";
import {
  historicoSchema,
  type HistoricoFormData,
  type HistoricoFormInput,
} from "@/features/historico/schemas/historico-schema";
import type { DisciplinaDto, HistoricoDto } from "@/types/api";
import { SEMESTRES } from "@/config/constants";
import { notificarSucesso } from "@/services/notification";

interface HistoricoFormProps {
  lancamento?: HistoricoDto | null;
  onSalvo: () => void;
  onCancelar: () => void;
}

/** Formulário de lançamento de disciplina cursada (histórico). */
export function HistoricoForm({ lancamento, onSalvo, onCancelar }: HistoricoFormProps) {
  const [erro, setErro] = useState<string | null>(null);
  const [disciplinas, setDisciplinas] = useState<DisciplinaDto[]>([]);
  const edicao = Boolean(lancamento);

  useEffect(() => {
    disciplinasApi
      .listar()
      .then(setDisciplinas)
      .catch((e) => setErro(obterMensagemDeErro(e)));
  }, []);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<HistoricoFormInput, unknown, HistoricoFormData>({
    resolver: zodResolver(historicoSchema),
    defaultValues: lancamento
      ? {
          disciplinaId: lancamento.disciplinaId,
          ano: lancamento.ano,
          semestre: lancamento.semestre,
          periodo: lancamento.periodo,
          mediaFinal: lancamento.mediaFinal,
        }
      : { ano: new Date().getFullYear(), semestre: 1, periodo: 1 },
  });

  async function onSubmit(dados: HistoricoFormData) {
    setErro(null);
    try {
      if (lancamento) {
        await historicoApi.atualizar(lancamento.id, dados);
        notificarSucesso("Lançamento atualizado.");
      } else {
        await historicoApi.criar(dados);
        notificarSucesso("Disciplina lançada no histórico.");
      }
      onSalvo();
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
      {erro && <Alert variant="destructive">{erro}</Alert>}

      <FormField id="disciplinaId" label="Disciplina" erro={errors.disciplinaId?.message}>
        <Select id="disciplinaId" {...register("disciplinaId")}>
          <option value="">Selecione...</option>
          {disciplinas.map((d) => (
            <option key={d.id} value={d.id}>
              {d.codigo} — {d.nome} ({d.creditos} cr.)
            </option>
          ))}
        </Select>
      </FormField>

      <div className="grid gap-4 sm:grid-cols-2">
        <FormField id="ano" label="Ano" erro={errors.ano?.message}>
          <Input id="ano" type="number" {...register("ano")} />
        </FormField>
        <FormField id="semestre" label="Semestre" erro={errors.semestre?.message}>
          <Select id="semestre" {...register("semestre")}>
            {SEMESTRES.map((s) => (
              <option key={s} value={s}>
                {s}º semestre
              </option>
            ))}
          </Select>
        </FormField>
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <FormField id="periodo" label="Período do aluno" erro={errors.periodo?.message}>
          <Input id="periodo" type="number" min={1} max={20} {...register("periodo")} />
        </FormField>
        <FormField id="mediaFinal" label="Média final" erro={errors.mediaFinal?.message}>
          <Input id="mediaFinal" type="number" step="0.1" min={0} max={10} {...register("mediaFinal")} />
        </FormField>
      </div>

      <div className="flex justify-end gap-2 pt-2">
        <Button variant="outline" onClick={onCancelar} disabled={isSubmitting}>
          Cancelar
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Salvando..." : edicao ? "Salvar alterações" : "Lançar"}
        </Button>
      </div>
    </form>
  );
}
