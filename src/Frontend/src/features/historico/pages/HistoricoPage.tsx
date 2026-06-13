import { useCallback, useEffect, useState } from "react";
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
import { HistoricoForm } from "@/features/historico/components/HistoricoForm";
import { historicoApi } from "@/features/historico/services/historico-api";
import type { CrDto, HistoricoDto } from "@/types/api";
import { formatarCr, formatarMedia } from "@/lib/formatters";
import { notificarErro, notificarSucesso } from "@/services/notification";

export function HistoricoPage() {
  const [lancamentos, setLancamentos] = useState<HistoricoDto[]>([]);
  const [cr, setCr] = useState<CrDto | null>(null);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const [formAberto, setFormAberto] = useState(false);
  const [emEdicao, setEmEdicao] = useState<HistoricoDto | null>(null);
  const [paraExcluir, setParaExcluir] = useState<HistoricoDto | null>(null);
  const [excluindo, setExcluindo] = useState(false);

  const recarregar = useCallback(async () => {
    setCarregando(true);
    setErro(null);
    try {
      const [lista, crDto] = await Promise.all([historicoApi.listar(), historicoApi.obterCr()]);
      setLancamentos(lista);
      setCr(crDto);
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    } finally {
      setCarregando(false);
    }
  }, []);

  useEffect(() => {
    void recarregar();
  }, [recarregar]);

  async function confirmarExclusao() {
    if (!paraExcluir) return;
    setExcluindo(true);
    try {
      await historicoApi.remover(paraExcluir.id);
      notificarSucesso("Lançamento removido.");
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
          <h1 className="text-2xl font-semibold">Histórico acadêmico</h1>
          <p className="text-sm text-muted-foreground">
            Disciplinas cursadas e médias finais — somente as suas.
          </p>
        </div>
        <div className="flex items-center gap-3">
          {cr && (
            <Badge variant="outline" className="px-3 py-1.5 text-sm">
              CR: <span className="ml-1 font-bold text-primary">{formatarCr(cr.cr)}</span>
            </Badge>
          )}
          <Button
            onClick={() => {
              setEmEdicao(null);
              setFormAberto(true);
            }}
          >
            <Plus className="h-4 w-4" />
            Lançar disciplina
          </Button>
        </div>
      </div>

      {carregando ? (
        <Loading mensagem="Carregando histórico..." />
      ) : erro ? (
        <ErrorState mensagem={erro} onTentarNovamente={recarregar} />
      ) : lancamentos.length === 0 ? (
        <EmptyState
          titulo="Nenhuma disciplina cursada ainda"
          descricao="Lance as disciplinas que você já cursou para acompanhar seu CR."
        />
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Disciplina</TableHead>
              <TableHead className="hidden md:table-cell">Professor</TableHead>
              <TableHead className="text-center">Ano/Sem.</TableHead>
              <TableHead className="text-center">Créditos</TableHead>
              <TableHead className="text-center">Média</TableHead>
              <TableHead className="w-24 text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {lancamentos.map((h) => (
              <TableRow key={h.id}>
                <TableCell>
                  <p className="font-medium">{h.disciplinaNome}</p>
                  <p className="font-mono text-xs text-muted-foreground">{h.disciplinaCodigo}</p>
                </TableCell>
                <TableCell className="hidden text-muted-foreground md:table-cell">
                  {h.disciplinaProfessor ?? "—"}
                </TableCell>
                <TableCell className="text-center">
                  {h.ano}/{h.semestre}
                </TableCell>
                <TableCell className="text-center">{h.creditos}</TableCell>
                <TableCell className="text-center">
                  <Badge variant={h.mediaFinal >= 6 ? "success" : "destructive"}>
                    {formatarMedia(h.mediaFinal)}
                  </Badge>
                </TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-1">
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => {
                        setEmEdicao(h);
                        setFormAberto(true);
                      }}
                      aria-label={`Editar lançamento de ${h.disciplinaCodigo}`}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="text-destructive hover:text-destructive"
                      onClick={() => setParaExcluir(h)}
                      aria-label={`Remover lançamento de ${h.disciplinaCodigo}`}
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
        titulo={emEdicao ? "Editar lançamento" : "Lançar disciplina cursada"}
      >
        <HistoricoForm
          lancamento={emEdicao}
          onSalvo={() => {
            setFormAberto(false);
            void recarregar();
          }}
          onCancelar={() => setFormAberto(false)}
        />
      </Dialog>

      <ConfirmDialog
        aberto={paraExcluir !== null}
        titulo="Remover lançamento"
        descricao={`Remover "${paraExcluir?.disciplinaNome}" (${paraExcluir?.ano}/${paraExcluir?.semestre}) do seu histórico?`}
        textoConfirmar="Remover"
        carregando={excluindo}
        onConfirmar={confirmarExclusao}
        onCancelar={() => setParaExcluir(null)}
      />
    </div>
  );
}
