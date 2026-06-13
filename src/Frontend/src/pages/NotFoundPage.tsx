import { Link } from "react-router-dom";
import { Compass } from "lucide-react";
import { Button } from "@/components/ui/button";
import { paths } from "@/routes/paths";

export function NotFoundPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 p-4 text-center">
      <Compass className="h-16 w-16 text-muted-foreground/50" />
      <h1 className="text-3xl font-bold">404</h1>
      <p className="text-muted-foreground">A página que você procura não existe.</p>
      <Button variant="outline">
        <Link to={paths.dashboard}>Voltar ao painel</Link>
      </Button>
    </div>
  );
}
