import { Link, useRouteError } from "react-router-dom";
import { Bug } from "lucide-react";
import { Button } from "@/components/ui/button";
import { paths } from "@/routes/paths";

/** Página de erro do roteador (erros de renderização não tratados). */
export function ErrorPage() {
  const erro = useRouteError();
  // útil em desenvolvimento; nada sensível é exibido ao usuário
  console.error(erro);

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 p-4 text-center">
      <Bug className="h-16 w-16 text-destructive/60" />
      <h1 className="text-3xl font-bold">Algo deu errado</h1>
      <p className="max-w-md text-muted-foreground">
        Ocorreu um erro inesperado na aplicação. Tente recarregar a página.
      </p>
      <div className="flex gap-2">
        <Button variant="outline" onClick={() => window.location.reload()}>
          Recarregar
        </Button>
        <Button variant="ghost">
          <Link to={paths.splash}>Voltar ao início</Link>
        </Button>
      </div>
    </div>
  );
}
