import { AlertTriangle } from "lucide-react";
import { Button } from "@/components/ui/button";

interface ErrorStateProps {
  mensagem: string;
  onTentarNovamente?: () => void;
}

export function ErrorState({ mensagem, onTentarNovamente }: ErrorStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 rounded-lg border border-destructive/30 bg-destructive/5 py-12 text-center">
      <AlertTriangle className="h-10 w-10 text-destructive" />
      <p className="max-w-md text-sm text-destructive">{mensagem}</p>
      {onTentarNovamente && (
        <Button variant="outline" size="sm" onClick={onTentarNovamente}>
          Tentar novamente
        </Button>
      )}
    </div>
  );
}
