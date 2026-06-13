import type { ReactNode } from "react";
import { Inbox } from "lucide-react";

interface EmptyStateProps {
  titulo: string;
  descricao?: string;
  acao?: ReactNode;
}

export function EmptyState({ titulo, descricao, acao }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border py-16 text-center">
      <Inbox className="h-10 w-10 text-muted-foreground/60" />
      <p className="font-medium">{titulo}</p>
      {descricao && <p className="max-w-sm text-sm text-muted-foreground">{descricao}</p>}
      {acao && <div className="mt-3">{acao}</div>}
    </div>
  );
}
