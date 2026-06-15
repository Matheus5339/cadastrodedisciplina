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
  const [pos, setPos] = useState(1); // 1 = Capa; 2..N+1 = páginas de figurinhas
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

  const figsDaPagina = useMemo(
    () => (dados?.figurinhas ?? []).filter((f) => f.pagina === pos - 1).sort((a, b) => a.numero - b.numero),
    [dados, pos],
  );

  if (erro) return <ErrorState mensagem={erro} onTentarNovamente={carregar} />;
  if (!dados) return <Loading mensagem="Carregando álbum..." />;

  const totalPaginas = dados.album.paginas + 1; // Capa (001) + páginas de figurinhas
  const adquiridas = dados.figurinhas.filter((f) => f.adquirida).length;

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
        <div className="flex items-center justify-between border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">{dados.album.nome}</h1>
          <span className="text-xs text-muted-foreground">
            {pos === 1 ? "Capa" : `Página ${pos - 1}`} · {adquiridas}/{dados.figurinhas.length} adquiridas
          </span>
        </div>

        <div className="flex min-h-[26rem] items-center justify-center p-4">
          {pos === 1 ? (
            // Página Capa (PDF §10) — "Foto da Capa"
            dados.album.possuiCapa ? (
              <ImagemAuth
                src={colecaoApi.capaUrl}
                alt={`Capa de ${dados.album.nome}`}
                className="aspect-[3/4] h-full max-h-[24rem] rounded-lg border border-border"
              />
            ) : (
              <div className="flex aspect-[3/4] h-full max-h-[24rem] items-center justify-center rounded-lg border border-dashed border-border px-10 text-sm text-muted-foreground">
                Foto da Capa
              </div>
            )
          ) : figsDaPagina.length === 0 ? (
            <p className="text-sm text-muted-foreground">Nenhuma figurinha nesta página.</p>
          ) : (
            <div className="grid w-full grid-cols-2 gap-3 sm:grid-cols-4">
              {figsDaPagina.map((f) =>
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

        {/* barra de paginação (PDF §10/§11): |< < NNN de NN > >| */}
        <div className="border-t border-border px-4 py-2.5">
          <Paginacao atual={pos} total={totalPaginas} onMudar={setPos} />
        </div>
      </div>

      {detalhe !== null && <FigurinhaDetailDialog id={detalhe} onFechar={() => setDetalhe(null)} />}
    </div>
  );
}
