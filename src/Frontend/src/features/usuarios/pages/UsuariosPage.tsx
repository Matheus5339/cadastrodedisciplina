import { useEffect, useState } from "react";
import { KeyRound, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ConfirmDialog } from "@/components/feedback/ConfirmDialog";
import { EmptyState } from "@/components/feedback/EmptyState";
import { ErrorState } from "@/components/feedback/ErrorState";
import { Loading } from "@/components/feedback/Loading";
import { UsuarioFormDialog } from "@/features/usuarios/components/UsuarioFormDialog";
import { usuariosApi } from "@/features/usuarios/services/usuarios-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarErro, notificarSucesso } from "@/services/notification";
import { useDebounce } from "@/hooks/useDebounce";
import { cn } from "@/lib/utils";
import type { UsuarioDto } from "@/types/api";

export function UsuariosPage() {
  const [filtro, setFiltro] = useState("");
  const filtroDeb = useDebounce(filtro, 300);
  const [lista, setLista] = useState<UsuarioDto[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [sel, setSel] = useState<UsuarioDto | null>(null);
  const [form, setForm] = useState<{ aberto: boolean; usuario: UsuarioDto | null }>({ aberto: false, usuario: null });
  const [removendo, setRemovendo] = useState<UsuarioDto | null>(null);
  const [excluindo, setExcluindo] = useState(false);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      const dados = await usuariosApi.listar(filtroDeb || undefined);
      setLista(dados);
      setSel((s) => (s && dados.some((u) => u.id === s.id) ? s : null));
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    void carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtroDeb]);

  async function confirmarRemocao() {
    if (!removendo) return;
    setExcluindo(true);
    try {
      await usuariosApi.remover(removendo.id);
      notificarSucesso("Usuário removido.");
      setRemovendo(null);
      setSel(null);
      void carregar();
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    } finally {
      setExcluindo(false);
    }
  }

  async function resetarSenha(u: UsuarioDto) {
    try {
      const { senha } = await usuariosApi.resetarSenha(u.id);
      notificarSucesso(`Senha de "${u.login}" redefinida para: ${senha}`);
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  return (
    <div className="mx-auto max-w-3xl space-y-3">
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">Usuários</h1>
        </div>

        {/* toolbar (PDF §6): + inserir · − excluir · E editar · (zerar senha) · filtro + F */}
        <div className="flex flex-wrap items-center gap-2 border-b border-border px-3 py-2">
          <div className="flex items-center gap-1">
            <Button size="icon" className="h-8 w-8" title="Inserir usuário" onClick={() => setForm({ aberto: true, usuario: null })}>
              <Plus className="h-4 w-4" />
            </Button>
            <Button variant="outline" size="icon" className="h-8 w-8" title="Excluir selecionado" disabled={!sel} onClick={() => sel && setRemovendo(sel)}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
            <Button variant="outline" size="icon" className="h-8 w-8" title="Editar selecionado" disabled={!sel} onClick={() => sel && setForm({ aberto: true, usuario: sel })}>
              <Pencil className="h-4 w-4" />
            </Button>
            <Button variant="outline" size="icon" className="h-8 w-8" title="Zerar senha do selecionado" disabled={!sel} onClick={() => sel && resetarSenha(sel)}>
              <KeyRound className="h-4 w-4" />
            </Button>
          </div>
          <div className="ml-auto flex items-center gap-1">
            <Input placeholder="filtrar por login..." value={filtro} onChange={(e) => setFiltro(e.target.value)} className="h-8 w-48" />
            <Button variant="outline" size="icon" className="h-8 w-8" title="Filtrar">
              <Search className="h-4 w-4" />
            </Button>
          </div>
        </div>

        {carregando ? (
          <div className="p-4"><Loading mensagem="Carregando usuários..." /></div>
        ) : erro ? (
          <div className="p-4"><ErrorState mensagem={erro} onTentarNovamente={carregar} /></div>
        ) : lista.length === 0 ? (
          <div className="p-4"><EmptyState titulo="Nenhum usuário" descricao="Use o botão + para criar o primeiro." /></div>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Login</TableHead>
                <TableHead>Perfil</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {lista.map((u) => (
                <TableRow
                  key={u.id}
                  onClick={() => setSel(u)}
                  onDoubleClick={() => setForm({ aberto: true, usuario: u })}
                  className={cn("cursor-pointer", sel?.id === u.id && "bg-accent/60")}
                >
                  <TableCell className="font-medium">{u.login}</TableCell>
                  <TableCell>{u.perfil}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>

      {form.aberto && (
        <UsuarioFormDialog
          usuario={form.usuario}
          onFechar={() => setForm({ aberto: false, usuario: null })}
          onSalvo={() => {
            setForm({ aberto: false, usuario: null });
            void carregar();
          }}
        />
      )}

      <ConfirmDialog
        aberto={removendo !== null}
        titulo="Remover usuário"
        descricao={`Tem certeza que deseja remover "${removendo?.login}"?`}
        textoConfirmar="Remover"
        carregando={excluindo}
        onConfirmar={confirmarRemocao}
        onCancelar={() => setRemovendo(null)}
      />
    </div>
  );
}
