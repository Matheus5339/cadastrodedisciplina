import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { Check, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Alert } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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
    <div className="mx-auto max-w-xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Adquirir figurinha</h1>
        <p className="text-sm text-muted-foreground">Informe a tag da figurinha para adicioná-la ao seu álbum.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Tag</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <form onSubmit={consultar} className="flex items-end gap-2">
            <div className="flex-1">
              <FormField id="tag" label="Tag (hash MD5)">
                <Input id="tag" value={tag} onChange={(e) => setTag(e.target.value)} placeholder="ex.: ff4adf...22345" className="font-mono" required />
              </FormField>
            </div>
            <Button type="submit" variant="outline" disabled={buscando || !tag.trim()}>
              <Search className="h-4 w-4" /> {buscando ? "..." : "Buscar"}
            </Button>
          </form>

          {erro && <Alert variant="destructive">{erro}</Alert>}

          {fig && (
            <div className="grid gap-4 rounded-lg border border-border p-4 sm:grid-cols-[140px_1fr]">
              <ImagemAuth src={colecaoApi.imagemUrl(fig.id)} alt={fig.nome} className="aspect-square w-full rounded-lg border border-border" carregar={fig.possuiImagem} />
              <div className="space-y-2">
                <p className="text-lg font-semibold">#{fig.numero} {fig.nome}</p>
                <p className="text-sm text-muted-foreground">Página {fig.pagina}</p>
                {fig.descricao && <p className="text-sm">{fig.descricao}</p>}
                <Button onClick={inserir} disabled={inserindo}>
                  <Check className="h-4 w-4" /> {inserindo ? "Inserindo..." : "Inserir no álbum"}
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
