import { ChevronFirst, ChevronLast, ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";

/** Calcula a janela de números visíveis em torno da página atual (com reticências). */
function janela(atual: number, total: number, raio = 2): (number | "…")[] {
  const set = new Set<number>([1, total]);
  for (let p = atual - raio; p <= atual + raio; p++) if (p >= 1 && p <= total) set.add(p);
  const ordenadas = [...set].filter((p) => p >= 1 && p <= total).sort((a, b) => a - b);
  const saida: (number | "…")[] = [];
  let anterior = 0;
  for (const p of ordenadas) {
    if (anterior && p - anterior > 1) saida.push("…");
    saida.push(p);
    anterior = p;
  }
  return saida;
}

/**
 * Barra de paginação no formato do PDF (§10/§11): |< < [1] [2] … [N] > >|
 * com botões de página clicáveis. "atual" e "total" são 1-based.
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
    <div className="flex flex-wrap items-center justify-center gap-1">
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noInicio} onClick={() => ir(1)} title="Primeira">
        <ChevronFirst className="h-4 w-4" />
      </Button>
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noInicio} onClick={() => ir(atual - 1)} title="Anterior">
        <ChevronLeft className="h-4 w-4" />
      </Button>

      {janela(atual, total).map((p, idx) =>
        p === "…" ? (
          <span key={`e${idx}`} className="px-1 text-sm text-muted-foreground">…</span>
        ) : (
          <Button
            key={p}
            variant={p === atual ? "default" : "outline"}
            size="icon"
            className="h-8 w-8 tabular-nums"
            onClick={() => ir(p)}
            title={`Página ${p}`}
          >
            {p}
          </Button>
        ),
      )}

      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noFim} onClick={() => ir(atual + 1)} title="Próxima">
        <ChevronRight className="h-4 w-4" />
      </Button>
      <Button variant="outline" size="icon" className="h-8 w-8" disabled={noFim} onClick={() => ir(total)} title="Última">
        <ChevronLast className="h-4 w-4" />
      </Button>
    </div>
  );
}
