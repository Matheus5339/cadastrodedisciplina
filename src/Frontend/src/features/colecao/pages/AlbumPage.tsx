import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ChevronLeft, ChevronRight, Lock, PlusSquare } from "lucide-react";
import { Button, buttonVariants } from "@/components/ui/button";
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
  const [pagina, setPagina] = useState(1);
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
    () => (dados?.figurinhas ?? []).filter((f) => f.pagina === pagina).sort((a, b) => a.numero - b.numero),
    [dados, pagina],
  );

  if (erro) return <ErrorState mensagem={erro} onTentarNovamente={carregar} />;
  if (!dados) return <Loading mensagem="Carregando álbum..." />;

  const totalAdquiridas = dados.figurinhas.filter((f) => f.adquirida).length;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold">{dados.album.nome}</h1>
          <p className="text-sm text-muted-foreground">
            {totalAdquiridas} de {dados.figurinhas.length} figurinhas adquiridas
          </p>
        </div>
        <Link to={paths.novaFigurinha} className={buttonVariants({})}>
          <PlusSquare className="h-4 w-4" /> Adquirir figurinha
        </Link>
      </div>

      {/* navegação de páginas */}
      <div className="flex items-center justify-center gap-3">
        <Button variant="outline" size="icon" disabled={pagina <= 1} onClick={() => setPagina((p) => p - 1)}>
          <ChevronLeft className="h-4 w-4" />
        </Button>
        <span className="text-sm font-medium">
          Página {pagina} de {dados.album.paginas}
        </span>
        <Button variant="outline" size="icon" disabled={pagina >= dados.album.paginas} onClick={() => setPagina((p) => p + 1)}>
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>

      {figsDaPagina.length === 0 ? (
        <p className="py-16 text-center text-sm text-muted-foreground">Nenhuma figurinha nesta página.</p>
      ) : (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4">
          {figsDaPagina.map((f) =>
            f.adquirida ? (
              <button
                key={f.id}
                type="button"
                onDoubleClick={() => setDetalhe(f.id)}
                className="group overflow-hidden rounded-lg border border-border bg-card text-left transition-shadow hover:shadow-md"
                title="Duplo clique para detalhes"
              >
                <ImagemAuth src={colecaoApi.imagemUrl(f.id)} alt={f.nome} className="aspect-square w-full" carregar={f.possuiImagem} />
                <div className="p-2">
                  <p className="truncate text-sm font-medium">#{f.numero} {f.nome}</p>
                </div>
              </button>
            ) : (
              <div
                key={f.id}
                className="flex aspect-square flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border bg-muted/40 text-muted-foreground"
                title="Figurinha ainda não adquirida"
              >
                <Lock className="h-6 w-6" />
                <span className="text-xs font-medium">#{f.numero}</span>
              </div>
            ),
          )}
        </div>
      )}

      {detalhe !== null && <FigurinhaDetailDialog id={detalhe} onFechar={() => setDetalhe(null)} />}
    </div>
  );
}
