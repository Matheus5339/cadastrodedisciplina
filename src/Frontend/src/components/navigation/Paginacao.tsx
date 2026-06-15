import { ChevronFirst, ChevronLast, ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";

/**
 * Barra de paginação no formato do PDF (§10/§11): |< < NNN de NN > >|.
 * "atual" e "total" são 1-based.
 */
export function Paginacao({
  atual,
  total,
  onMudar,
}: {
  atual: number;
  total: number;
  onMudar: (pagina: number) => void;
}) {
  const ir = (p: number) => onMudar(Math.min(total, Math.max(1, p)));
  const noInicio = atual <= 1;
  const noFim = atual >= total;

  return (
    <div className="flex items-center justify-center gap-1">
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noInicio} onClick={() => ir(1)} title="Primeira">
        <ChevronFirst className="h-4 w-4" />
      </Button>
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noInicio} onClick={() => ir(atual - 1)} title="Anterior">
        <ChevronLeft className="h-4 w-4" />
      </Button>
      <span className="min-w-[6.5rem] text-center text-sm font-medium tabular-nums">
        {String(atual).padStart(3, "0")} de {total}
      </span>
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noFim} onClick={() => ir(atual + 1)} title="Próxima">
        <ChevronRight className="h-4 w-4" />
      </Button>
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noFim} onClick={() => ir(total)} title="Última">
        <ChevronLast className="h-4 w-4" />
      </Button>
    </div>
  );
}
