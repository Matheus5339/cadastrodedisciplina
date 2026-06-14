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
import type { UsuarioDto } from "@/types/api";

export function UsuariosPage() {
  const [filtro, setFiltro] = useState("");
  const filtroDeb = useDebounce(filtro, 300);
  const [lista, setLista] = useState<UsuarioDto[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [form, setForm] = useState<{ aberto: boolean; usuario: UsuarioDto | null }>({ aberto: false, usuario: null });
  const [removendo, setRemovendo] = useState<UsuarioDto | null>(null);
  const [excluindo, setExcluindo] = useState(false);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      setLista(await usuariosApi.listar(filtroDeb || undefined));
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
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold">Usuários</h1>
          <p className="text-sm text-muted-foreground">Gerencie os usuários do sistema.</p>
        </div>
        <Button onClick={() => setForm({ aberto: true, usuario: null })}>
          <Plus className="h-4 w-4" /> Novo usuário
        </Button>
      </div>

      <div className="relative max-w-sm">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          className="pl-9"
          placeholder="Filtrar por login..."
          value={filtro}
          onChange={(e) => setFiltro(e.target.value)}
        />
      </div>

      {carregando ? (
        <Loading mensagem="Carregando usuários..." />
      ) : erro ? (
        <ErrorState mensagem={erro} onTentarNovamente={carregar} />
      ) : lista.length === 0 ? (
        <EmptyState titulo="Nenhum usuário" descricao="Crie o primeiro usuário no botão acima." />
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Login</TableHead>
              <TableHead>Perfil</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {lista.map((u) => (
              <TableRow key={u.id}>
                <TableCell className="font-medium">{u.login}</TableCell>
                <TableCell>
                  <span className="rounded-full bg-accent px-2.5 py-0.5 text-xs font-medium text-accent-foreground">
                    {u.perfil}
                  </span>
                </TableCell>
                <TableCell>
                  <div className="flex justify-end gap-1">
                    <Button variant="ghost" size="sm" onClick={() => resetarSenha(u)} title="Zerar senha">
                      <KeyRound className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="sm" onClick={() => setForm({ aberto: true, usuario: u })} title="Editar">
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="sm" onClick={() => setRemovendo(u)} title="Remover">
                      <Trash2 className="h-4 w-4 text-destructive" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}

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
