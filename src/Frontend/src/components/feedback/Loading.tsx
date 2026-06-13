import { Spinner } from "@/components/ui/spinner";

export function Loading({ mensagem = "Carregando..." }: { mensagem?: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 py-16 text-muted-foreground">
      <Spinner className="h-8 w-8" />
      <p className="text-sm">{mensagem}</p>
    </div>
  );
}
