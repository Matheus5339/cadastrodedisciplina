import { BookImage, Info, LogOut, Menu, PlusSquare, Sticker, User, Users, X } from "lucide-react";
import { NavLink } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/core/auth/useAuth";
import { useSessao } from "@/core/session/useSessao";
import { useUiStore } from "@/stores/ui-store";
import { cn } from "@/lib/utils";
import { paths } from "@/routes/paths";
import type { Perfil } from "@/types/api";

type Item = { to: string; rotulo: string; Icone: typeof Users };

const itensPorPerfil: Record<Perfil, Item[]> = {
  Administrador: [{ to: paths.usuarios, rotulo: "Usuários", Icone: Users }],
  Autor: [{ to: paths.autoria, rotulo: "Autoria", Icone: BookImage }],
  Colecionador: [
    { to: paths.album, rotulo: "Álbum", Icone: BookImage },
    { to: paths.novaFigurinha, rotulo: "Nova figurinha", Icone: PlusSquare },
  ],
};

function LinkNav({ to, rotulo, Icone, onClick }: Item & { onClick?: () => void }) {
  return (
    <NavLink
      to={to}
      end
      onClick={onClick}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-2 rounded-full px-3.5 py-2 text-sm font-medium transition-colors",
          isActive ? "bg-white/20 text-white shadow-sm" : "text-white/75 hover:bg-white/10 hover:text-white",
        )
      }
    >
      <Icone className="h-4 w-4" />
      {rotulo}
    </NavLink>
  );
}

export function TopNav() {
  const { usuario, perfil } = useSessao();
  const { logout } = useAuth();
  const { sidebarAberta: menuAberto, alternarSidebar: alternarMenu, fecharSidebar: fecharMenu } = useUiStore();

  const itens: Item[] = perfil ? itensPorPerfil[perfil] : [];
  const comuns: Item[] = [
    { to: paths.conta, rotulo: "Conta", Icone: User },
    { to: paths.sobre, rotulo: "Sobre", Icone: Info },
  ];
  const todos = [...itens, ...comuns];

  return (
    <header className="sticky top-0 z-40 bg-gradient-to-r from-teal-700 via-teal-600 to-emerald-600 text-white shadow-md">
      <div className="mx-auto flex h-16 w-full max-w-6xl items-center justify-between gap-4 px-4">
        <div className="flex items-center gap-2.5">
          <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-white/15 ring-1 ring-white/25">
            <Sticker className="h-5 w-5" />
          </div>
          <div className="leading-tight">
            <p className="text-sm font-semibold tracking-tight">Álbum de Figurinhas</p>
            <p className="text-[11px] text-white/70">{perfil}</p>
          </div>
        </div>

        <nav className="hidden items-center gap-1 md:flex">
          {todos.map((i) => (
            <LinkNav key={i.to} {...i} />
          ))}
        </nav>

        <div className="hidden items-center gap-3 md:flex">
          <span className="max-w-[10rem] truncate text-sm font-medium">{usuario?.login}</span>
          <Button variant="ghost" size="sm" onClick={logout} className="text-white hover:bg-white/15 hover:text-white">
            <LogOut className="h-4 w-4" />
            <span>Sair</span>
          </Button>
        </div>

        <Button
          variant="ghost"
          size="icon"
          className="text-white hover:bg-white/15 hover:text-white md:hidden"
          onClick={alternarMenu}
          aria-label="Abrir menu"
        >
          {menuAberto ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
        </Button>
      </div>

      {menuAberto && (
        <div className="border-t border-white/15 bg-teal-700/95 px-4 pb-4 pt-2 md:hidden">
          <nav className="flex flex-col gap-1">
            {todos.map((i) => (
              <LinkNav key={i.to} {...i} onClick={fecharMenu} />
            ))}
          </nav>
          <div className="mt-3 flex items-center justify-between border-t border-white/15 pt-3">
            <span className="text-sm font-medium">{usuario?.login}</span>
            <Button variant="ghost" size="sm" onClick={logout} className="text-white hover:bg-white/15 hover:text-white">
              <LogOut className="h-4 w-4" />
              Sair
            </Button>
          </div>
        </div>
      )}
    </header>
  );
}
