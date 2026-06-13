import { X } from "lucide-react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { useNotificationStore } from "@/services/notification";

/** Exibe as notificações globais (alertas) no canto da tela. */
export function Toaster() {
  const { notificacoes, remover } = useNotificationStore();
  if (notificacoes.length === 0) return null;

  return (
    <div className="fixed bottom-4 right-4 z-[60] flex w-80 flex-col gap-2">
      {notificacoes.map((n) => (
        <div key={n.id} className="relative">
          <Alert variant={n.tipo === "erro" ? "destructive" : "success"} className="pr-10 shadow-lg">
            {n.mensagem}
          </Alert>
          <Button
            variant="ghost"
            size="icon"
            className="absolute right-1 top-1 h-7 w-7"
            onClick={() => remover(n.id)}
            aria-label="Fechar notificação"
          >
            <X className="h-3.5 w-3.5" />
          </Button>
        </div>
      ))}
    </div>
  );
}
