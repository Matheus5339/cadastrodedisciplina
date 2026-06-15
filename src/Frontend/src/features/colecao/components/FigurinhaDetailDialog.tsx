import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Dialog } from "@/components/ui/dialog";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { Loading } from "@/components/feedback/Loading";
import { colecaoApi } from "@/features/colecao/services/colecao-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import type { FigurinhaDto } from "@/types/api";

/** Detalhes de uma figurinha (duplo clique no álbum — PDF §11). */
export function FigurinhaDetailDialog({ id, onFechar }: { id: number; onFechar: () => void }) {
  const [fig, setFig] = useState<FigurinhaDto | null>(null);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    colecaoApi
      .obterFigurinha(id)
      .then(setFig)
      .catch((e) => setErro(obterMensagemDeErro(e)));
  }, [id]);

  return (
    <Dialog aberto onFechar={onFechar} titulo={fig ? `#${fig.numero} — ${fig.nome}` : "Figurinha"}>
      {erro ? (
        <p className="text-sm text-destructive">{erro}</p>
      ) : !fig ? (
        <Loading mensagem="Carregando..." />
      ) : (
        <div className="grid gap-4 sm:grid-cols-[180px_1fr]">
          <ImagemAuth src={colecaoApi.imagemUrl(fig.id)} alt={fig.nome} className="aspect-square w-full rounded-lg border border-border" carregar={fig.possuiImagem} />
          <div className="flex flex-col">
            <dl className="space-y-2 text-sm">
              <div><dt className="text-muted-foreground">Nome</dt><dd className="font-medium">{fig.nome}</dd></div>
              <div><dt className="text-muted-foreground">Número</dt><dd className="font-medium">{fig.numero}</dd></div>
              <div><dt className="text-muted-foreground">Página</dt><dd className="font-medium">{fig.pagina}</dd></div>
              {fig.descricao && <div><dt className="text-muted-foreground">Descrição</dt><dd>{fig.descricao}</dd></div>}
              <div><dt className="text-muted-foreground">Tag (MD5)</dt><dd className="break-all font-mono text-xs">{fig.tag}</dd></div>
            </dl>
            <div className="mt-4 flex justify-end">
              <Button type="button" variant="outline" onClick={onFechar}>Voltar</Button>
            </div>
          </div>
        </div>
      )}
    </Dialog>
  );
}
