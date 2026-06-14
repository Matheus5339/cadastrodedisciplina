import { Link } from "react-router-dom";
import { ArrowLeft, Sticker } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { buttonVariants } from "@/components/ui/button";
import { paths } from "@/routes/paths";

/** Tela "Sobre" (item da rubrica). */
export function SobrePage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-4">
      <Card className="w-full max-w-lg">
        <CardHeader className="items-center text-center">
          <div className="mx-auto mb-2 flex h-14 w-14 items-center justify-center rounded-2xl bg-primary/10 ring-1 ring-primary/20">
            <Sticker className="h-7 w-7 text-primary" />
          </div>
          <CardTitle>Sobre o Álbum de Figurinhas</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4 text-sm text-muted-foreground">
          <p>
            Sistema para simular um álbum de figurinhas virtual, desenvolvido para o trabalho de
            Programação Orientada a Objeto da UCP.
          </p>
          <div>
            <p className="font-medium text-foreground">Perfis</p>
            <ul className="mt-1 list-inside list-disc space-y-1">
              <li><span className="font-medium text-foreground">Administrador:</span> gerencia os usuários.</li>
              <li><span className="font-medium text-foreground">Autor:</span> cria e edita o álbum e as figurinhas.</li>
              <li><span className="font-medium text-foreground">Colecionador:</span> visualiza o álbum e adquire figurinhas pela tag.</li>
            </ul>
          </div>
          <p className="text-xs">
            Stack: .NET 10 · React 19 · SQLite · cada figurinha tem uma tag (hash MD5 da imagem).
          </p>
          <div className="pt-2 text-center">
            <Link to={paths.splash} className={buttonVariants({ variant: "outline" })}>
              <ArrowLeft className="h-4 w-4" /> Voltar
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
