import { Dialog } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";

interface ConfirmDialogProps {
  aberto: boolean;
  titulo: string;
  descricao: string;
  textoConfirmar?: string;
  carregando?: boolean;
  onConfirmar: () => void;
  onCancelar: () => void;
}

/** Confirmação de ações destrutivas (ex.: exclusão de disciplina). */
export function ConfirmDialog({
  aberto,
  titulo,
  descricao,
  textoConfirmar = "Excluir",
  carregando = false,
  onConfirmar,
  onCancelar,
}: ConfirmDialogProps) {
  return (
    <Dialog aberto={aberto} onFechar={onCancelar} titulo={titulo} descricao={descricao} className="max-w-md">
      <div className="flex justify-end gap-2">
        <Button variant="outline" onClick={onCancelar} disabled={carregando}>
          Cancelar
        </Button>
        <Button variant="destructive" onClick={onConfirmar} disabled={carregando}>
          {carregando ? "Excluindo..." : textoConfirmar}
        </Button>
      </div>
    </Dialog>
  );
}
