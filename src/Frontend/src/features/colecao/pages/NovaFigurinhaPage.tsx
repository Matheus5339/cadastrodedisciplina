import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Alert } from "@/components/ui/alert";
import { FormField } from "@/components/forms/FormField";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { colecaoApi } from "@/features/colecao/services/colecao-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarSucesso } from "@/services/notification";
import { paths } from "@/routes/paths";
import type { FigurinhaDto } from "@/types/api";

export function NovaFigurinhaPage() {
  const navigate = useNavigate();
  const [tag, setTag] = useState("");
  const [fig, setFig] = useState<FigurinhaDto | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [buscando, setBuscando] = useState(false);
  const [inserindo, setInserindo] = useState(false);

  async function consultar(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setFig(null);
    setBuscando(true);
    try {
      setFig(await colecaoApi.consultar(tag.trim()));
    } catch {
      setErro("Nenhuma figurinha encontrada com esta tag.");
    } finally {
      setBuscando(false);
    }
  }

  async function inserir() {
    if (!fig) return;
    setInserindo(true);
    try {
      await colecaoApi.adquirir(tag.trim());
      notificarSucesso(`Figurinha "${fig.nome}" adicionada ao seu álbum!`);
      navigate(paths.album);
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    } finally {
      setInserindo(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">Adquirir figurinha</h1>
        </div>

        <div className="space-y-4 p-4">
          {/* Tag + botão "..." (PDF §12) */}
          <form onSubmit={consultar} className="flex items-end gap-2">
            <div className="flex-1">
              <FormField id="tag" label="Tag">
                <Input id="tag" value={tag} onChange={(e) => setTag(e.target.value)} placeholder="informe a tag da figurinha" className="font-mono" required />
              </FormField>
            </div>
            <Button type="submit" variant="outline" className="font-mono" disabled={buscando || !tag.trim()} title="Buscar pela tag">
              {buscando ? "..." : "..."}
            </Button>
          </form>

          {erro && <Alert variant="destructive">{erro}</Alert>}

          {/* dados da figurinha + preview */}
          <div className="grid gap-4 sm:grid-cols-[1fr_180px]">
            <div className="space-y-3">
              <FormField id="nome" label="Nome">
                <Input id="nome" value={fig?.nome ?? ""} readOnly placeholder="—" />
              </FormField>
              <div className="grid grid-cols-2 gap-3">
                <FormField id="pagina" label="Página">
                  <Input id="pagina" value={fig?.pagina ?? ""} readOnly placeholder="—" />
                </FormField>
                <FormField id="numero" label="Número">
                  <Input id="numero" value={fig?.numero ?? ""} readOnly placeholder="—" />
                </FormField>
              </div>
            </div>
            <div>
              <p className="mb-1.5 text-xs font-medium text-muted-foreground">Preview</p>
              {fig?.possuiImagem ? (
                <ImagemAuth src={colecaoApi.imagemUrl(fig.id)} alt={fig.nome} className="aspect-square w-full rounded-lg border border-border" />
              ) : (
                <div className="flex aspect-square w-full items-center justify-center rounded-lg border border-dashed border-border text-xs text-muted-foreground">
                  Preview
                </div>
              )}
            </div>
          </div>

          {/* botões Inserir / Voltar (PDF §12) */}
          <div className="flex justify-end gap-2 border-t border-border pt-4">
            <Button type="button" variant="outline" onClick={() => navigate(paths.album)}>
              Voltar
            </Button>
            <Button onClick={inserir} disabled={!fig || inserindo}>
              {inserindo ? "Inserindo..." : "Inserir"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
