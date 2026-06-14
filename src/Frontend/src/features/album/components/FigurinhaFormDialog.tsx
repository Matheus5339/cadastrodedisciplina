import { useEffect, useState, type FormEvent } from "react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog } from "@/components/ui/dialog";
import { FormField } from "@/components/forms/FormField";
import { ImagemAuth } from "@/components/ui/imagem-auth";
import { figurinhasApi } from "@/features/album/services/album-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarSucesso } from "@/services/notification";
import type { FigurinhaDto } from "@/types/api";

interface Props {
  figurinha: FigurinhaDto | null; // null = nova
  totalPaginas: number;
  onFechar: () => void;
  onSalvo: () => void;
}

export function FigurinhaFormDialog({ figurinha, totalPaginas, onFechar, onSalvo }: Props) {
  const editando = figurinha !== null;
  const [numero, setNumero] = useState(figurinha?.numero ?? 1);
  const [nome, setNome] = useState(figurinha?.nome ?? "");
  const [pagina, setPagina] = useState(figurinha?.pagina ?? 1);
  const [descricao, setDescricao] = useState(figurinha?.descricao ?? "");
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [previewLocal, setPreviewLocal] = useState<string | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);

  useEffect(() => {
    if (!arquivo) {
      setPreviewLocal(null);
      return;
    }
    const url = URL.createObjectURL(arquivo);
    setPreviewLocal(url);
    return () => URL.revokeObjectURL(url);
  }, [arquivo]);

  async function salvar(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    if (!editando && !arquivo) {
      setErro("A imagem da figurinha é obrigatória.");
      return;
    }
    setSalvando(true);
    try {
      const fd = new FormData();
      fd.append("Numero", String(numero));
      fd.append("Nome", nome);
      fd.append("Pagina", String(pagina));
      fd.append("Descricao", descricao);
      if (arquivo) fd.append("Imagem", arquivo);

      if (editando) await figurinhasApi.atualizar(figurinha.id, fd);
      else await figurinhasApi.criar(fd);
      notificarSucesso(editando ? "Figurinha atualizada." : "Figurinha criada.");
      onSalvo();
    } catch (err) {
      setErro(obterMensagemDeErro(err));
    } finally {
      setSalvando(false);
    }
  }

  return (
    <Dialog aberto onFechar={onFechar} titulo={editando ? "Editar figurinha" : "Nova figurinha"} className="max-w-2xl">
      <form onSubmit={salvar} className="grid gap-4 sm:grid-cols-[1fr_180px]" noValidate>
        <div className="space-y-4">
          {erro && <Alert variant="destructive">{erro}</Alert>}
          <div className="grid grid-cols-2 gap-3">
            <FormField id="numero" label="Número">
              <Input id="numero" type="number" min={1} value={numero} onChange={(e) => setNumero(Number(e.target.value))} required />
            </FormField>
            <FormField id="pagina" label="Página">
              <Input id="pagina" type="number" min={1} max={totalPaginas} value={pagina} onChange={(e) => setPagina(Number(e.target.value))} required />
            </FormField>
          </div>
          <FormField id="nome" label="Nome">
            <Input id="nome" value={nome} onChange={(e) => setNome(e.target.value)} required maxLength={150} />
          </FormField>
          <FormField id="descricao" label="Descrição">
            <Input id="descricao" value={descricao} onChange={(e) => setDescricao(e.target.value)} maxLength={1000} />
          </FormField>
          <FormField id="imagem" label={editando ? "Trocar imagem (opcional)" : "Imagem"}>
            <Input id="imagem" type="file" accept="image/png,image/jpeg,image/webp" onChange={(e) => setArquivo(e.target.files?.[0] ?? null)} />
          </FormField>
          {editando && figurinha.tag && (
            <FormField id="tag" label="Tag (MD5 da imagem)">
              <Input id="tag" value={figurinha.tag} readOnly className="font-mono text-xs" />
            </FormField>
          )}
        </div>

        <div className="space-y-2">
          <p className="text-xs font-medium text-muted-foreground">Preview</p>
          {previewLocal ? (
            <img src={previewLocal} alt="Preview" className="aspect-square w-full rounded-lg border border-border object-cover" />
          ) : editando && figurinha.possuiImagem ? (
            <ImagemAuth src={figurinhasApi.imagemUrl(figurinha.id)} alt={figurinha.nome} className="aspect-square w-full rounded-lg border border-border" />
          ) : (
            <div className="flex aspect-square w-full items-center justify-center rounded-lg border border-dashed border-border text-xs text-muted-foreground">
              sem imagem
            </div>
          )}
        </div>

        <div className="flex justify-end gap-2 sm:col-span-2">
          <Button type="button" variant="outline" onClick={onFechar}>
            Cancelar
          </Button>
          <Button type="submit" disabled={salvando}>
            {salvando ? "Salvando..." : "Salvar"}
          </Button>
        </div>
      </form>
    </Dialog>
  );
}
