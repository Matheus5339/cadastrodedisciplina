import { Link } from "react-router-dom";
import { GraduationCap } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { LoginForm } from "@/features/auth/components/LoginForm";
import { paths } from "@/routes/paths";

export function LoginPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-4">
      <Card className="w-full max-w-md">
        <CardHeader className="items-center text-center">
          <GraduationCap className="mx-auto mb-2 h-10 w-10 text-primary" />
          <CardTitle>Controle de Disciplinas — UCP</CardTitle>
          <CardDescription>Entre para gerenciar suas disciplinas e acompanhar seu CR.</CardDescription>
        </CardHeader>
        <CardContent>
          <LoginForm />
          <p className="mt-4 text-center text-sm text-muted-foreground">
            Ainda não tem conta?{" "}
            <Link to={paths.cadastro} className="font-medium text-primary hover:underline">
              Cadastre-se
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
