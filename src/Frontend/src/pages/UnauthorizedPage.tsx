import { Link } from "react-router-dom";
import { ShieldAlert } from "lucide-react";
import { Button } from "@/components/ui/button";
import { paths } from "@/routes/paths";

export function UnauthorizedPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 p-4 text-center">
      <ShieldAlert className="h-16 w-16 text-destructive/60" />
      <h1 className="text-3xl font-bold">Acesso não autorizado</h1>
      <p className="max-w-md text-muted-foreground">
        Você não tem permissão para acessar este recurso. Entre com sua conta para continuar.
      </p>
      <Button variant="outline">
        <Link to={paths.login}>Ir para o login</Link>
      </Button>
    </div>
  );
}
