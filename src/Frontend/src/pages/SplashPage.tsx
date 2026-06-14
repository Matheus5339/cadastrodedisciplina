import { Link } from "react-router-dom";
import { Info, LogIn, Sticker } from "lucide-react";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { paths } from "@/routes/paths";

/** Tela de splash / abertura (item da rubrica). */
export function SplashPage() {
  return (
    <div className="relative flex min-h-screen flex-col items-center justify-center overflow-hidden bg-gradient-to-br from-teal-700 via-teal-600 to-emerald-600 p-6 text-center text-white">
      <div className="pointer-events-none absolute -left-32 -top-32 h-96 w-96 rounded-full bg-white/10 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-32 -right-32 h-96 w-96 rounded-full bg-emerald-300/20 blur-3xl" />

      <div className="relative flex flex-col items-center gap-6">
        <div className="flex h-24 w-24 items-center justify-center rounded-3xl bg-white/15 ring-1 ring-white/30">
          <Sticker className="h-12 w-12" />
        </div>
        <div>
          <h1 className="text-4xl font-bold tracking-tight">Álbum de Figurinhas</h1>
          <p className="mt-2 text-white/80">Colecione, gerencie e complete o seu álbum.</p>
        </div>
        <div className="flex flex-wrap items-center justify-center gap-3">
          <Link to={paths.login} className={buttonVariants({ variant: "secondary", size: "lg" })}>
            <LogIn className="h-4 w-4" /> Entrar
          </Link>
          <Link
            to={paths.sobre}
            className={cn(buttonVariants({ variant: "ghost", size: "lg" }), "text-white hover:bg-white/15 hover:text-white")}
          >
            <Info className="h-4 w-4" /> Sobre
          </Link>
        </div>
      </div>
      <p className="absolute bottom-6 text-xs text-white/60">UCP · Programação Orientada a Objeto</p>
    </div>
  );
}
