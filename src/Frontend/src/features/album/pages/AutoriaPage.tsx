import { useEffect, useRef, useState, type FormEvent } from "react";
import { Download, Eraser, FileText, Pencil, Plus, Search, Trash2, Upload } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ConfirmDialog } from "@/components/feedback/ConfirmDialog";
import { EmptyState } from "@/components/feedback/EmptyState";
import { Loading } from "@/components/feedback/Loading";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { FigurinhaFormDialog } from "@/features/album/components/FigurinhaFormDialog";
import { albumApi, arquivosApi, figurinhasApi } from "@/features/album/services/album-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarErro, notificarSucesso } from "@/services/notification";
import { useDebounce } from "@/hooks/useDebounce";
import { cn } from "@/lib/utils";
import type { AlbumDto, FigurinhaDto } from "@/types/api";

function baixar(blob: Blob, nome: string) {
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = nome;
  a.click();
  URL.revokeObjectURL(url);
}

export function AutoriaPage() {
  const [album, setAlbum] = useState<AlbumDto | null>(null);
  const [nome, setNome] = useState("");
  const [paginas, setPaginas] = useState(1);
  const [capaNome, setCapaNome] = useState("");
  const [capaVersao, setCapaVersao] = useState(0);
  const capaRef = useRef<HTMLInputElement>(null);

  const [filtro, setFiltro] = useState("");
  const filtroDeb = useDebounce(filtro, 300);
  const [figs, setFigs] = useState<FigurinhaDto[]>([]);
  const [sel, setSel] = useState<FigurinhaDto | null>(null);
  const [carregando, setCarregando] = useState(true);
  const [form, setForm] = useState<{ aberto: boolean; figurinha: FigurinhaDto | null }>({ aberto: false, figurinha: null });
  const [removendo, setRemovendo] = useState<FigurinhaDto | null>(null);
  const [limparAberto, setLimparAberto] = useState(false);
  const importRef = useRef<HTMLInputElement>(null);

  async function carregarAlbum() {
    try {
      const a = await albumApi.obter();
      setAlbum(a);
      setNome(a.nome);
      setPaginas(a.paginas);
      setCapaNome(a.possuiCapa ? "capa atual" : "");
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  async function carregarFigs() {
    setCarregando(true);
    try {
      const dados = await figurinhasApi.listar(filtroDeb || undefined);
      setFigs(dados);
      setSel((s) => (s && dados.some((f) => f.id === s.id) ? s : null));
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    void carregarAlbum();
  }, []);
  useEffect(() => {
    void carregarFigs();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtroDeb]);

  async function salvarAlbum(e: FormEvent) {
    e.preventDefault();
    try {
      setAlbum(await albumApi.atualizar(nome, paginas));
      notificarSucesso("Álbum atualizado.");
    } catch (err) {
      notificarErro(obterMensagemDeErro(err));
    }
  }

  async function enviarCapa(arquivo: File | undefined) {
    if (!arquivo) return;
    try {
      await albumApi.enviarCapa(arquivo);
      setCapaNome(arquivo.name);
      setCapaVersao((v) => v + 1);
      setAlbum((a) => (a ? { ...a, possuiCapa: true } : a));
      notificarSucesso("Capa atualizada.");
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  async function confirmarRemocao() {
    if (!removendo) return;
    try {
      await figurinhasApi.remover(removendo.id);
      notificarSucesso("Figurinha removida.");
      setRemovendo(null);
      setSel(null);
      void carregarFigs();
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  async function limparTudo() {
    try {
      await figurinhasApi.limpar();
      notificarSucesso("Todas as figurinhas foram removidas.");
      setLimparAberto(false);
      void carregarFigs();
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  async function exportar(tipo: "texto" | "binario") {
    try {
      const blob = tipo === "texto" ? await arquivosApi.exportarTexto() : await arquivosApi.exportarBinario();
      baixar(blob, tipo === "texto" ? "figurinhas.txt" : "figurinhas.figb");
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  async function importar(arquivo: File | undefined) {
    if (!arquivo) return;
    try {
      const { importadas } = await arquivosApi.importarBinario(arquivo);
      notificarSucesso(`${importadas} figurinha(s) importada(s).`);
      void carregarFigs();
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    } finally {
      if (importRef.current) importRef.current.value = "";
    }
  }

  return (
    <div className="mx-auto max-w-4xl space-y-3">
      {/* Janela FrmAutoria — dados do álbum */}
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">Autoria do álbum</h1>
        </div>
        <div className="grid gap-4 p-4 sm:grid-cols-[1fr_140px]">
          <form onSubmit={salvarAlbum} className="space-y-3">
            <div className="space-y-1.5">
              <label className="text-sm font-medium" htmlFor="album-nome">Nome</label>
              <Input id="album-nome" value={nome} onChange={(e) => setNome(e.target.value)} maxLength={150} />
            </div>
            <div className="space-y-1.5">
              <label className="text-sm font-medium" htmlFor="album-pag">Páginas</label>
              <Input id="album-pag" type="number" min={1} value={paginas} onChange={(e) => setPaginas(Number(e.target.value))} />
            </div>
            <div className="space-y-1.5">
              <label className="text-sm font-medium">Capa</label>
              <div className="flex gap-1">
                <Input value={capaNome} readOnly placeholder="nenhuma capa" />
                <Button type="button" variant="outline" onClick={() => capaRef.current?.click()} title="Escolher capa">...</Button>
                <input ref={capaRef} type="file" accept="image/png,image/jpeg,image/webp" className="hidden" onChange={(e) => enviarCapa(e.target.files?.[0])} />
              </div>
            </div>
            <Button type="submit" size="sm">Salvar álbum</Button>
          </form>
          <div>
            {album?.possuiCapa ? (
              <ImagemAuth key={capaVersao} src={albumApi.capaUrl} alt="Capa" className="aspect-[3/4] w-full rounded-lg border border-border" />
            ) : (
              <div className="flex aspect-[3/4] w-full items-center justify-center rounded-lg border border-dashed border-border text-xs text-muted-foreground">
                sem capa
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Grupo Figurinhas */}
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="border-b border-border px-4 py-2.5">
          <h2 className="text-base font-semibold">Figurinhas</h2>
        </div>

        {/* toolbar (PDF §8): + − E [filtro] F L */}
        <div className="flex flex-wrap items-center gap-2 border-b border-border px-3 py-2">
          <div className="flex items-center gap-1">
            <Button size="icon" className="h-8 w-8" title="Inserir figurinha" onClick={() => setForm({ aberto: true, figurinha: null })}>
              <Plus className="h-4 w-4" />
            </Button>
            <Button variant="outline" size="icon" className="h-8 w-8" title="Excluir selecionada" disabled={!sel} onClick={() => sel && setRemovendo(sel)}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
            <Button variant="outline" size="icon" className="h-8 w-8" title="Editar selecionada" disabled={!sel} onClick={() => sel && setForm({ aberto: true, figurinha: sel })}>
              <Pencil className="h-4 w-4" />
            </Button>
            <Button variant="outline" size="icon" className="h-8 w-8" title="Limpar (remover todas)" disabled={figs.length === 0} onClick={() => setLimparAberto(true)}>
              <Eraser className="h-4 w-4" />
            </Button>
          </div>
          <div className="ml-auto flex items-center gap-1">
            <Input placeholder="filtrar por nome ou tag..." value={filtro} onChange={(e) => setFiltro(e.target.value)} className="h-8 w-56" />
            <Button variant="outline" size="icon" className="h-8 w-8" title="Filtrar">
              <Search className="h-4 w-4" />
            </Button>
          </div>
        </div>

        {/* arquivo texto/binário (rubrica) */}
        <div className="flex flex-wrap items-center gap-2 border-b border-border bg-muted/30 px-3 py-2 text-xs">
          <span className="font-medium text-muted-foreground">Arquivo:</span>
          <Button variant="outline" size="sm" className="h-7" onClick={() => exportar("texto")}>
            <FileText className="h-3.5 w-3.5" /> Exportar texto
          </Button>
          <Button variant="outline" size="sm" className="h-7" onClick={() => exportar("binario")}>
            <Download className="h-3.5 w-3.5" /> Exportar binário
          </Button>
          <label className="inline-flex h-7 cursor-pointer items-center gap-1.5 rounded-md border border-border bg-card px-2.5 hover:bg-accent">
            <Upload className="h-3.5 w-3.5" /> Importar binário
            <input ref={importRef} type="file" accept=".figb,application/octet-stream" className="hidden" onChange={(e) => importar(e.target.files?.[0])} />
          </label>
        </div>

        {carregando ? (
          <div className="p-4"><Loading mensagem="Carregando figurinhas..." /></div>
        ) : figs.length === 0 ? (
          <div className="p-4"><EmptyState titulo="Nenhuma figurinha" descricao="Use o botão + para adicionar a primeira." /></div>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-20">#</TableHead>
                <TableHead>Nome</TableHead>
                <TableHead className="w-24">Página</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {figs.map((f) => (
                <TableRow
                  key={f.id}
                  onClick={() => setSel(f)}
                  onDoubleClick={() => setForm({ aberto: true, figurinha: f })}
                  className={cn("cursor-pointer", sel?.id === f.id && "bg-accent/60")}
                >
                  <TableCell className="font-medium tabular-nums">{String(f.numero).padStart(3, "0")}</TableCell>
                  <TableCell>{f.nome}</TableCell>
                  <TableCell>{f.pagina}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>

      {form.aberto && (
        <FigurinhaFormDialog
          figurinha={form.figurinha}
          totalPaginas={album?.paginas ?? 1}
          onFechar={() => setForm({ aberto: false, figurinha: null })}
          onSalvo={() => {
            setForm({ aberto: false, figurinha: null });
            void carregarFigs();
          }}
        />
      )}

      <ConfirmDialog
        aberto={removendo !== null}
        titulo="Remover figurinha"
        descricao={`Remover a figurinha "${removendo?.nome}"?`}
        textoConfirmar="Remover"
        onConfirmar={confirmarRemocao}
        onCancelar={() => setRemovendo(null)}
      />
      <ConfirmDialog
        aberto={limparAberto}
        titulo="Limpar figurinhas"
        descricao="Isso remove TODAS as figurinhas do álbum. Tem certeza?"
        textoConfirmar="Limpar tudo"
        onConfirmar={limparTudo}
        onCancelar={() => setLimparAberto(false)}
      />
    </div>
  );
}
