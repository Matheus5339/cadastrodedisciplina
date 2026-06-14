import { Link } from "react-router-dom";
import { Sticker } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { LoginForm } from "@/features/auth/components/LoginForm";
import { paths } from "@/routes/paths";

export function LoginPage() {
  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-gradient-to-br from-teal-700 via-teal-600 to-emerald-600 p-4">
      <div className="pointer-events-none absolute -left-24 -top-24 h-72 w-72 rounded-full bg-white/10 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-24 -right-24 h-72 w-72 rounded-full bg-emerald-300/20 blur-3xl" />

      <Card className="w-full max-w-md shadow-2xl">
        <CardHeader className="items-center text-center">
          <div className="mx-auto mb-2 flex h-14 w-14 items-center justify-center rounded-2xl bg-primary/10 ring-1 ring-primary/20">
            <Sticker className="h-7 w-7 text-primary" />
          </div>
          <CardTitle>Álbum de Figurinhas</CardTitle>
          <CardDescription>Entre com seu login e senha.</CardDescription>
        </CardHeader>
        <CardContent>
          <LoginForm />
          <p className="mt-4 text-center text-sm text-muted-foreground">
            <Link to={paths.sobre} className="font-medium text-primary hover:underline">
              Sobre o sistema
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
