import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Info, KeyRound, Lock, PlusSquare } from "lucide-react";
import { Loading } from "@/components/feedback/Loading";
import { ErrorState } from "@/components/feedback/ErrorState";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { Paginacao } from "@/components/navigation/Paginacao";
import { FigurinhaDetailDialog } from "@/features/colecao/components/FigurinhaDetailDialog";
import { colecaoApi } from "@/features/colecao/services/colecao-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { paths } from "@/routes/paths";
import type { AlbumColecionadorDto } from "@/types/api";

export function AlbumPage() {
  const [dados, setDados] = useState<AlbumColecionadorDto | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [i, setI] = useState(0); // índice do item atual (1 por vez: capa OU figurinha)
  const [detalhe, setDetalhe] = useState<number | null>(null);

  async function carregar() {
    setErro(null);
    try {
      setDados(await colecaoApi.meuAlbum());
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    }
  }

  useEffect(() => {
    void carregar();
  }, []);

  // sequência: por país -> capa do país, depois suas figurinhas (tudo 1 por vez)
  const itens = useMemo(
    () => (dados?.figurinhas ?? []).slice().sort((a, b) => a.pagina - b.pagina || a.numero - b.numero),
    [dados],
  );
  // a 1ª figurinha de cada país é a "capa do país"
  const capaIdPorPagina = useMemo(() => {
    const m = new Map<number, number>();
    for (const f of itens) if (!m.has(f.pagina)) m.set(f.pagina, f.id);
    return m;
  }, [itens]);
  const nomePaisPorPagina = useMemo(() => {
    const m = new Map<number, string>();
    for (const f of itens) if (!m.has(f.pagina)) m.set(f.pagina, f.nome);
    return m;
  }, [itens]);

  if (erro) return <ErrorState mensagem={erro} onTentarNovamente={carregar} />;
  if (!dados) return <Loading mensagem="Carregando álbum..." />;

  const adquiridas = dados.figurinhas.filter((f) => f.adquirida).length;
  const idx = Math.min(i, Math.max(0, itens.length - 1));
  const atual = itens[idx];
  const ehCapa = atual ? capaIdPorPagina.get(atual.pagina) === atual.id : false;
  const pais = atual ? nomePaisPorPagina.get(atual.pagina) ?? "" : "";

  return (
    <div className="mx-auto max-w-3xl space-y-3">
      {/* tsbMenu (PDF §10): 1 Trocar senha · 2 Adquirir figurinha · 3 Sobre */}
      <div className="flex flex-wrap items-center gap-1.5">
        <Link to={paths.conta} className="inline-flex items-center gap-1.5 rounded-md border border-border bg-card px-3 py-1.5 text-sm font-medium hover:bg-accent">
          <KeyRound className="h-4 w-4" /> Trocar senha
        </Link>
        <Link to={paths.novaFigurinha} className="inline-flex items-center gap-1.5 rounded-md border border-border bg-card px-3 py-1.5 text-sm font-medium hover:bg-accent">
          <PlusSquare className="h-4 w-4" /> Adquirir figurinha
        </Link>
        <Link to={paths.sobre} className="inline-flex items-center gap-1.5 rounded-md border border-border bg-card px-3 py-1.5 text-sm font-medium hover:bg-accent">
          <Info className="h-4 w-4" /> Sobre
        </Link>
      </div>

      {/* janela FrmAlbum: 1 item por página (capa do país ou figurinha) */}
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="flex items-center justify-between border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">
            {dados.album.nome}
            {pais ? <span className="text-muted-foreground"> — {pais}</span> : null}
          </h1>
          <span className="text-xs text-muted-foreground">
            {ehCapa ? "Capa do país" : atual ? `Figurinha ${String(atual.numero).padStart(3, "0")}` : ""} · {adquiridas}/{dados.figurinhas.length}
          </span>
        </div>

        <div className="flex min-h-[28rem] items-center justify-center p-6">
          {!atual ? (
            <p className="text-sm text-muted-foreground">Álbum vazio.</p>
          ) : ehCapa ? (
            // Capa do país (bandeira/seleção)
            <div className="flex flex-col items-center gap-3">
              <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Foto da Capa</p>
              <ImagemAuth src={colecaoApi.imagemUrl(atual.id)} alt={atual.nome} className="aspect-[3/4] max-h-[24rem] rounded-lg border border-border shadow-sm" />
              <p className="text-lg font-semibold">{atual.nome}</p>
            </div>
          ) : atual.adquirida ? (
            // Figurinha adquirida (duplo clique abre os detalhes)
            <button
              type="button"
              onDoubleClick={() => setDetalhe(atual.id)}
              title="Duplo clique para ver os detalhes"
              className="flex flex-col items-center gap-3"
            >
              <ImagemAuth src={colecaoApi.imagemUrl(atual.id)} alt={atual.nome} className="aspect-[3/4] max-h-[24rem] rounded-lg border border-border shadow-sm" carregar={atual.possuiImagem} />
              <p className="text-xs text-muted-foreground">duplo clique para detalhes</p>
            </button>
          ) : (
            // Figurinha ainda não adquirida
            <div className="flex aspect-[3/4] max-h-[24rem] w-64 flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border bg-muted/40 text-muted-foreground">
              <Lock className="h-8 w-8" />
              <span className="text-sm font-medium">Figurinha {String(atual.numero).padStart(3, "0")}</span>
              <span className="text-xs">ainda não adquirida</span>
            </div>
          )}
        </div>

        {/* paginação com botões clicáveis (1 por vez): |< < [1][2]…[N] > >| */}
        <div className="border-t border-border px-4 py-2.5">
          <Paginacao atual={idx + 1} total={Math.max(1, itens.length)} onMudar={(p) => setI(p - 1)} />
        </div>
      </div>

      {detalhe !== null && <FigurinhaDetailDialog id={detalhe} onFechar={() => setDetalhe(null)} />}
    </div>
  );
}
