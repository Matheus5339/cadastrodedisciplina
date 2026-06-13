import { useMemo, useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Dialog } from "@/components/ui/dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ConfirmDialog } from "@/components/feedback/ConfirmDialog";
import { EmptyState } from "@/components/feedback/EmptyState";
import { ErrorState } from "@/components/feedback/ErrorState";
import { Loading } from "@/components/feedback/Loading";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { useDebounce } from "@/hooks/useDebounce";
import {
  DisciplinaFilters,
  type FiltrosState,
} from "@/features/disciplinas/components/DisciplinaFilters";
import { DisciplinaForm } from "@/features/disciplinas/components/DisciplinaForm";
import { useDisciplinas } from "@/features/disciplinas/hooks/useDisciplinas";
import { disciplinasApi } from "@/features/disciplinas/services/disciplinas-api";
import type { DisciplinaDto } from "@/types/api";
import { notificarErro, notificarSucesso } from "@/services/notification";

export function DisciplinasPage() {
  const [filtros, setFiltros] = useState<FiltrosState>({ nome: "", professor: "", ano: "", semestre: "" });
  const nomeDebounced = useDebounce(filtros.nome);
  const professorDebounced = useDebounce(filtros.professor);

  const filtrosApi = useMemo(
    () => ({
      nome: nomeDebounced || undefined,
      professor: professorDebounced || undefined,
      ano: filtros.ano ? Number(filtros.ano) : undefined,
      semestre: filtros.semestre ? Number(filtros.semestre) : undefined,
    }),
    [nomeDebounced, professorDebounced, filtros.ano, filtros.semestre],
  );

  const { disciplinas, carregando, erro, recarregar } = useDisciplinas(filtrosApi);

  const [formAberto, setFormAberto] = useState(false);
  const [emEdicao, setEmEdicao] = useState<DisciplinaDto | null>(null);
  const [paraExcluir, setParaExcluir] = useState<DisciplinaDto | null>(null);
  const [excluindo, setExcluindo] = useState(false);

  function abrirCriacao() {
    setEmEdicao(null);
    setFormAberto(true);
  }

  function abrirEdicao(disciplina: DisciplinaDto) {
    setEmEdicao(disciplina);
    setFormAberto(true);
  }

  async function confirmarExclusao() {
    if (!paraExcluir) return;
    setExcluindo(true);
    try {
      await disciplinasApi.remover(paraExcluir.id);
      notificarSucesso("Disciplina excluída.");
      setParaExcluir(null);
      await recarregar();
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    } finally {
      setExcluindo(false);
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold">Disciplinas</h1>
          <p className="text-sm text-muted-foreground">Catálogo de disciplinas do curso.</p>
        </div>
        <Button onClick={abrirCriacao}>
          <Plus className="h-4 w-4" />
          Nova disciplina
        </Button>
      </div>

      <DisciplinaFilters filtros={filtros} onChange={setFiltros} />

      {carregando ? (
        <Loading mensagem="Carregando disciplinas..." />
      ) : erro ? (
        <ErrorState mensagem={erro} onTentarNovamente={recarregar} />
      ) : disciplinas.length === 0 ? (
        <EmptyState
          titulo="Nenhuma disciplina encontrada"
          descricao="Ajuste os filtros ou cadastre uma nova disciplina."
          acao={
            <Button variant="outline" size="sm" onClick={abrirCriacao}>
              <Plus className="h-4 w-4" /> Cadastrar disciplina
            </Button>
          }
        />
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Código</TableHead>
              <TableHead>Nome</TableHead>
              <TableHead className="hidden md:table-cell">Professor</TableHead>
              <TableHead className="text-center">Período</TableHead>
              <TableHead className="text-center">Créditos</TableHead>
              <TableHead className="w-24 text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {disciplinas.map((d) => (
              <TableRow key={d.id}>
                <TableCell className="font-mono text-xs">{d.codigo}</TableCell>
                <TableCell className="font-medium">{d.nome}</TableCell>
                <TableCell className="hidden text-muted-foreground md:table-cell">
                  {d.professor ?? "—"}
                </TableCell>
                <TableCell className="text-center">
                  <Badge variant="secondary">{d.periodo}º</Badge>
                </TableCell>
                <TableCell className="text-center">{d.creditos}</TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-1">
                    <Button variant="ghost" size="icon" onClick={() => abrirEdicao(d)} aria-label={`Editar ${d.codigo}`}>
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="text-destructive hover:text-destructive"
                      onClick={() => setParaExcluir(d)}
                      aria-label={`Excluir ${d.codigo}`}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}

      <Dialog
        aberto={formAberto}
        onFechar={() => setFormAberto(false)}
        titulo={emEdicao ? "Editar disciplina" : "Nova disciplina"}
      >
        <DisciplinaForm
          disciplina={emEdicao}
          onSalva={() => {
            setFormAberto(false);
            void recarregar();
          }}
          onCancelar={() => setFormAberto(false)}
        />
      </Dialog>

      <ConfirmDialog
        aberto={paraExcluir !== null}
        titulo="Excluir disciplina"
        descricao={`Tem certeza que deseja excluir "${paraExcluir?.nome}"? Esta ação não pode ser desfeita.`}
        carregando={excluindo}
        onConfirmar={confirmarExclusao}
        onCancelar={() => setParaExcluir(null)}
      />
    </div>
  );
}
