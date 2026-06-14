import { useEffect, useRef, useState, type FormEvent } from "react";
import { Download, Eraser, FileText, Image as ImageIcon, Pencil, Plus, Trash2, Upload } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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
  const [capaVersao, setCapaVersao] = useState(0);

  const [filtro, setFiltro] = useState("");
  const filtroDeb = useDebounce(filtro, 300);
  const [figs, setFigs] = useState<FigurinhaDto[]>([]);
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
    } catch (e) {
      notificarErro(obterMensagemDeErro(e));
    }
  }

  async function carregarFigs() {
    setCarregando(true);
    try {
      setFigs(await figurinhasApi.listar(filtroDeb || undefined));
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
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold">Autoria do álbum</h1>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Álbum</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid gap-4 sm:grid-cols-[1fr_140px]">
            <form onSubmit={salvarAlbum} className="space-y-3">
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1.5">
                  <label className="text-sm font-medium" htmlFor="album-nome">Nome</label>
                  <Input id="album-nome" value={nome} onChange={(e) => setNome(e.target.value)} maxLength={150} />
                </div>
                <div className="space-y-1.5">
                  <label className="text-sm font-medium" htmlFor="album-pag">Páginas</label>
                  <Input id="album-pag" type="number" min={1} value={paginas} onChange={(e) => setPaginas(Number(e.target.value))} />
                </div>
              </div>
              <div className="flex items-center gap-2">
                <Button type="submit" size="sm">Salvar álbum</Button>
                <label className="inline-flex cursor-pointer items-center gap-2 rounded-md border border-border bg-card px-3 py-1.5 text-sm hover:bg-accent">
                  <ImageIcon className="h-4 w-4" /> Trocar capa
                  <input type="file" accept="image/png,image/jpeg,image/webp" className="hidden" onChange={(e) => enviarCapa(e.target.files?.[0])} />
                </label>
              </div>
            </form>
            <div>
              {album?.possuiCapa ? (
                <ImagemAuth key={capaVersao} src={albumApi.capaUrl} alt="Capa" className="aspect-square w-full rounded-lg border border-border" />
              ) : (
                <div className="flex aspect-square w-full items-center justify-center rounded-lg border border-dashed border-border text-xs text-muted-foreground">
                  sem capa
                </div>
              )}
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 className="text-lg font-semibold">Figurinhas</h2>
        <div className="flex flex-wrap gap-2">
          <Button variant="outline" size="sm" onClick={() => exportar("texto")}>
            <FileText className="h-4 w-4" /> Exportar texto
          </Button>
          <Button variant="outline" size="sm" onClick={() => exportar("binario")}>
            <Download className="h-4 w-4" /> Exportar binário
          </Button>
          <label className="inline-flex cursor-pointer items-center gap-2 rounded-md border border-border bg-card px-3 text-sm hover:bg-accent">
            <Upload className="h-4 w-4" /> Importar binário
            <input ref={importRef} type="file" accept=".figb,application/octet-stream" className="hidden" onChange={(e) => importar(e.target.files?.[0])} />
          </label>
          <Button variant="outline" size="sm" onClick={() => setLimparAberto(true)} disabled={figs.length === 0}>
            <Eraser className="h-4 w-4" /> Limpar
          </Button>
          <Button size="sm" onClick={() => setForm({ aberto: true, figurinha: null })}>
            <Plus className="h-4 w-4" /> Nova figurinha
          </Button>
        </div>
      </div>

      <Input placeholder="Filtrar por nome ou tag..." value={filtro} onChange={(e) => setFiltro(e.target.value)} className="max-w-sm" />

      {carregando ? (
        <Loading mensagem="Carregando figurinhas..." />
      ) : figs.length === 0 ? (
        <EmptyState titulo="Nenhuma figurinha" descricao="Adicione a primeira figurinha do álbum." />
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-16">#</TableHead>
              <TableHead className="w-16">Img</TableHead>
              <TableHead>Nome</TableHead>
              <TableHead className="w-16">Pág.</TableHead>
              <TableHead>Tag</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {figs.map((f) => (
              <TableRow key={f.id}>
                <TableCell className="font-medium">{f.numero}</TableCell>
                <TableCell>
                  <ImagemAuth src={figurinhasApi.imagemUrl(f.id)} alt={f.nome} className="h-9 w-9 rounded" carregar={f.possuiImagem} />
                </TableCell>
                <TableCell>{f.nome}</TableCell>
                <TableCell>{f.pagina}</TableCell>
                <TableCell className="max-w-[12rem] truncate font-mono text-xs text-muted-foreground">{f.tag}</TableCell>
                <TableCell>
                  <div className="flex justify-end gap-1">
                    <Button variant="ghost" size="sm" onClick={() => setForm({ aberto: true, figurinha: f })} title="Editar">
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="sm" onClick={() => setRemovendo(f)} title="Remover">
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
