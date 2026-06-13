import { Link } from "react-router-dom";
import { GraduationCap } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { RegisterForm } from "@/features/auth/components/RegisterForm";
import { paths } from "@/routes/paths";

export function RegisterPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-4">
      <Card className="w-full max-w-lg">
        <CardHeader className="items-center text-center">
          <GraduationCap className="mx-auto mb-2 h-10 w-10 text-primary" />
          <CardTitle>Criar conta</CardTitle>
          <CardDescription>Cadastre-se com um e-mail válido para controlar suas disciplinas.</CardDescription>
        </CardHeader>
        <CardContent>
          <RegisterForm />
          <p className="mt-4 text-center text-sm text-muted-foreground">
            Já tem conta?{" "}
            <Link to={paths.login} className="font-medium text-primary hover:underline">
              Entrar
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
