import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ChevronFirst, ChevronLast, ChevronLeft, ChevronRight, Info, KeyRound, Lock, PlusSquare } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Loading } from "@/components/feedback/Loading";
import { ErrorState } from "@/components/feedback/ErrorState";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { FigurinhaDetailDialog } from "@/features/colecao/components/FigurinhaDetailDialog";
import { colecaoApi } from "@/features/colecao/services/colecao-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { paths } from "@/routes/paths";
import type { AlbumColecionadorDto } from "@/types/api";

export function AlbumPage() {
  const [dados, setDados] = useState<AlbumColecionadorDto | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [i, setI] = useState(0); // índice global do item atual (1 por vez: capa OU figurinha)
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

  // sequência global: por país -> capa do país, depois suas figurinhas (1 por vez)
  const itens = useMemo(
    () => (dados?.figurinhas ?? []).slice().sort((a, b) => a.pagina - b.pagina || a.numero - b.numero),
    [dados],
  );

  if (erro) return <ErrorState mensagem={erro} onTentarNovamente={carregar} />;
  if (!dados) return <Loading mensagem="Carregando álbum..." />;
  if (itens.length === 0) return <p className="py-16 text-center text-sm text-muted-foreground">Álbum vazio.</p>;

  const idx = Math.min(i, itens.length - 1);
  const atual = itens[idx];

  // contexto do país atual (numeração POR PAÍS: capa = 1, depois as figurinhas)
  const inicioPais = itens.findIndex((f) => f.pagina === atual.pagina);
  const itensPais = itens.filter((f) => f.pagina === atual.pagina);
  const totalPais = itensPais.length;
  const posLocal = idx - inicioPais + 1; // 1..totalPais
  const ehCapa = posLocal === 1; // a 1ª do país é a capa do país
  const nomePais = itensPais[0]?.nome ?? "";
  const adquiridas = dados.figurinhas.filter((f) => f.adquirida).length;

  // páginas do álbum (cada país é uma página): [{ pagina, inicio (índice global) }]
  const paginas: { pagina: number; inicio: number }[] = [];
  {
    const vistos = new Set<number>();
    itens.forEach((f, gi) => {
      if (!vistos.has(f.pagina)) {
        vistos.add(f.pagina);
        paginas.push({ pagina: f.pagina, inicio: gi });
      }
    });
  }

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

      {/* janela FrmAlbum: 1 item por página */}
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="flex items-center justify-between border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">
            {dados.album.nome}
            {nomePais ? <span className="text-muted-foreground"> — {nomePais}</span> : null}
          </h1>
          <span className="text-xs text-muted-foreground">
            {ehCapa ? "Capa do país" : `Figurinha ${String(atual.numero).padStart(3, "0")}`} · {adquiridas}/{dados.figurinhas.length}
          </span>
        </div>

        {/* botões de PÁGINA do álbum (ACIMA) — cada país é uma página: 1, 2, 3, ... */}
        <div className="flex flex-wrap items-center justify-center gap-1 border-b border-border px-4 py-2">
          {paginas.map(({ pagina, inicio }, n) => (
            <Button
              key={pagina}
              variant={pagina === atual.pagina ? "default" : "outline"}
              size="icon"
              className="h-8 w-8 tabular-nums"
              onClick={() => setI(inicio)}
              title={`Página ${n + 1}`}
            >
              {n + 1}
            </Button>
          ))}
        </div>

        {/* o item (1 por vez) */}
        <div className="flex min-h-[26rem] items-center justify-center p-6">
          {ehCapa ? (
            <div className="flex flex-col items-center gap-3">
              <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Foto da Capa</p>
              <ImagemAuth src={colecaoApi.imagemUrl(atual.id)} alt={atual.nome} className="aspect-[3/4] max-h-[22rem] rounded-lg border border-border shadow-sm" />
              <p className="text-lg font-semibold">{atual.nome}</p>
            </div>
          ) : atual.adquirida ? (
            <button type="button" onDoubleClick={() => setDetalhe(atual.id)} title="Duplo clique para ver os detalhes" className="flex flex-col items-center gap-3">
              <ImagemAuth src={colecaoApi.imagemUrl(atual.id)} alt={atual.nome} className="aspect-[3/4] max-h-[22rem] rounded-lg border border-border shadow-sm" carregar={atual.possuiImagem} />
              <p className="text-xs text-muted-foreground">duplo clique para detalhes</p>
            </button>
          ) : (
            <div className="flex aspect-[3/4] max-h-[22rem] w-60 flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border bg-muted/40 text-muted-foreground">
              <Lock className="h-8 w-8" />
              <span className="text-sm font-medium">Figurinha {String(atual.numero).padStart(3, "0")}</span>
              <span className="text-xs">ainda não adquirida</span>
            </div>
          )}
        </div>

        {/* contador (ABAIXO da figurinha) — formato do PDF: |< < N de Total > >| (por país) */}
        <div className="flex items-center justify-center gap-1 border-t border-border px-4 py-2.5">
          <Button variant="outline" size="icon" className="h-8 w-8" disabled={posLocal <= 1} onClick={() => setI(inicioPais)} title="Primeira do país">
            <ChevronFirst className="h-4 w-4" />
          </Button>
          <Button variant="outline" size="icon" className="h-8 w-8" disabled={idx <= 0} onClick={() => setI(Math.max(0, idx - 1))} title="Anterior">
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <span className="min-w-[6rem] text-center text-sm font-medium tabular-nums">
            {posLocal} de {totalPais}
          </span>
          <Button variant="outline" size="icon" className="h-8 w-8" disabled={idx >= itens.length - 1} onClick={() => setI(Math.min(itens.length - 1, idx + 1))} title="Próxima">
            <ChevronRight className="h-4 w-4" />
          </Button>
          <Button variant="outline" size="icon" className="h-8 w-8" disabled={posLocal >= totalPais} onClick={() => setI(inicioPais + totalPais - 1)} title="Última do país">
            <ChevronLast className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {detalhe !== null && <FigurinhaDetailDialog id={detalhe} onFechar={() => setDetalhe(null)} />}
    </div>
  );
}
