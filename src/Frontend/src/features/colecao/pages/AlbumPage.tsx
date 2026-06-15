import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Info, KeyRound, Lock, PlusSquare } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Loading } from "@/components/feedback/Loading";
import { ErrorState } from "@/components/feedback/ErrorState";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { Paginacao } from "@/components/navigation/Paginacao";
import { FigurinhaDetailDialog } from "@/features/colecao/components/FigurinhaDetailDialog";
import { colecaoApi } from "@/features/colecao/services/colecao-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { paths } from "@/routes/paths";
import type { AlbumColecionadorDto } from "@/types/api";

type FigAlbum = AlbumColecionadorDto["figurinhas"][number];

const POR_CONJUNTO = 8; // figurinhas por "conjunto" (grade), como no PDF (Fig 001..008)

export function AlbumPage() {
  const [dados, setDados] = useState<AlbumColecionadorDto | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [paisIdx, setPaisIdx] = useState(0); // país atual (botões de cima)
  const [sub, setSub] = useState(1); // 001 = capa do país; 002..M = conjuntos (contador de baixo)
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

  // agrupa por país (cada página = um país), figurinhas ordenadas pelo número
  const paises = useMemo(() => {
    const map = new Map<number, FigAlbum[]>();
    for (const f of dados?.figurinhas ?? []) {
      const arr = map.get(f.pagina) ?? [];
      arr.push(f);
      map.set(f.pagina, arr);
    }
    return [...map.entries()]
      .sort((a, b) => a[0] - b[0])
      .map(([pagina, figs]) => ({ pagina, figs: figs.slice().sort((a, b) => a.numero - b.numero) }));
  }, [dados]);

  if (erro) return <ErrorState mensagem={erro} onTentarNovamente={carregar} />;
  if (!dados) return <Loading mensagem="Carregando álbum..." />;
  if (paises.length === 0) return <p className="py-16 text-center text-sm text-muted-foreground">Álbum vazio.</p>;

  const pIdx = Math.min(paisIdx, paises.length - 1);
  const pais = paises[pIdx];
  const capa = pais.figs[0] ?? null; // capa do país (figurinha única)
  const jogadores = pais.figs.slice(1); // demais figurinhas do país
  const subTotal = 1 + Math.ceil(jogadores.length / POR_CONJUNTO); // 001 (capa) + conjuntos
  const s = Math.min(sub, subTotal);
  const grade = s >= 2 ? jogadores.slice((s - 2) * POR_CONJUNTO, (s - 2) * POR_CONJUNTO + POR_CONJUNTO) : [];
  const adquiridas = dados.figurinhas.filter((f) => f.adquirida).length;

  function trocarPais(k: number) {
    setPaisIdx(k);
    setSub(1); // volta para a capa do país escolhido
  }

  return (
    <div className="mx-auto max-w-4xl space-y-3">
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

      {/* janela FrmAlbum */}
      <div className="rounded-xl border border-border bg-card shadow-sm">
        {/* botões de PÁGINA = países (ACIMA): 1, 2, 3, ... */}
        <div className="flex flex-wrap items-center justify-center gap-1 border-b border-border px-4 py-2">
          {paises.map((p, k) => (
            <Button
              key={p.pagina}
              variant={k === pIdx ? "default" : "outline"}
              size="icon"
              className="h-8 w-8 tabular-nums"
              onClick={() => trocarPais(k)}
              title={`Página ${k + 1}${p.figs[0] ? ` — ${p.figs[0].nome}` : ""}`}
            >
              {k + 1}
            </Button>
          ))}
        </div>

        <div className="flex items-center justify-between border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">
            {dados.album.nome}
            {capa ? <span className="text-muted-foreground"> — {capa.nome}</span> : null}
          </h1>
          <span className="text-xs text-muted-foreground">
            {s === 1 ? "Capa do país" : "Figurinhas"} · {adquiridas}/{dados.figurinhas.length}
          </span>
        </div>

        <div className="flex min-h-[26rem] items-center justify-center p-4">
          {s === 1 ? (
            // 001 = capa do país (figurinha única)
            capa ? (
              <div className="flex flex-col items-center gap-3">
                <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Foto da Capa</p>
                <ImagemAuth src={colecaoApi.imagemUrl(capa.id)} alt={capa.nome} className="aspect-[3/4] max-h-[24rem] rounded-lg border border-border shadow-sm" />
                <p className="text-lg font-semibold">{capa.nome}</p>
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">Sem capa.</p>
            )
          ) : grade.length === 0 ? (
            <p className="text-sm text-muted-foreground">Nenhuma figurinha nesta página.</p>
          ) : (
            // 002+ = conjunto (grade) de figurinhas do país
            <div className="grid w-full grid-cols-2 gap-3 sm:grid-cols-4">
              {grade.map((f) =>
                f.adquirida ? (
                  <button
                    key={f.id}
                    type="button"
                    onDoubleClick={() => setDetalhe(f.id)}
                    title="Duplo clique para ver os detalhes"
                    className="group overflow-hidden rounded-lg border border-border bg-background text-left transition-shadow hover:shadow-md"
                  >
                    <ImagemAuth src={colecaoApi.imagemUrl(f.id)} alt={f.nome} className="aspect-square w-full" carregar={f.possuiImagem} />
                    <p className="truncate p-1.5 text-center text-xs font-medium">Fig {String(f.numero).padStart(3, "0")}</p>
                  </button>
                ) : (
                  <div
                    key={f.id}
                    title="Figurinha ainda não adquirida"
                    className="flex aspect-[4/5] flex-col items-center justify-center gap-1 rounded-lg border border-dashed border-border bg-muted/40 text-muted-foreground"
                  >
                    <Lock className="h-5 w-5" />
                    <span className="text-xs font-medium">Fig {String(f.numero).padStart(3, "0")}</span>
                  </div>
                ),
              )}
            </div>
          )}
        </div>

        {/* contador (ABAIXO) por país: |< < NNN de NN > >| (001 = capa, depois os conjuntos) */}
        <div className="border-t border-border px-4 py-2.5">
          <Paginacao atual={s} total={subTotal} onMudar={setSub} />
        </div>
      </div>

      {detalhe !== null && <FigurinhaDetailDialog id={detalhe} onFechar={() => setDetalhe(null)} />}
    </div>
  );
}
