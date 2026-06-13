import { BookOpen, GraduationCap, LayoutDashboard, LogOut, Menu, ScrollText, User, X } from "lucide-react";
import { NavLink } from "react-router-dom";
import { UserAvatar } from "@/components/layout/UserAvatar";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/core/auth/useAuth";
import { useSessao } from "@/core/session/useSessao";
import { useUiStore } from "@/stores/ui-store";
import { cn } from "@/lib/utils";
import { paths } from "@/routes/paths";

const itens = [
  { to: paths.dashboard, icone: LayoutDashboard, rotulo: "Painel" },
  { to: paths.disciplinas, icone: BookOpen, rotulo: "Disciplinas" },
  { to: paths.historico, icone: ScrollText, rotulo: "Histórico" },
  { to: paths.perfil, icone: User, rotulo: "Perfil" },
];

/** Item de navegação horizontal (pílula sobre a barra teal). */
function LinkNav({
  to,
  Icone,
  rotulo,
  onClick,
}: {
  to: string;
  Icone: typeof LayoutDashboard;
  rotulo: string;
  onClick?: () => void;
}) {
  return (
    <NavLink
      to={to}
      onClick={onClick}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-2 rounded-full px-3.5 py-2 text-sm font-medium transition-colors",
          isActive
            ? "bg-white/20 text-white shadow-sm"
            : "text-white/75 hover:bg-white/10 hover:text-white",
        )
      }
    >
      <Icone className="h-4 w-4" />
      {rotulo}
    </NavLink>
  );
}

/** Barra de navegação superior (substitui a sidebar). */
export function TopNav() {
  const { aluno } = useSessao();
  const { logout } = useAuth();
  const { sidebarAberta: menuAberto, alternarSidebar: alternarMenu, fecharSidebar: fecharMenu } = useUiStore();

  return (
    <header className="sticky top-0 z-40 bg-gradient-to-r from-teal-700 via-teal-600 to-emerald-600 text-white shadow-md">
      <div className="mx-auto flex h-16 w-full max-w-6xl items-center justify-between gap-4 px-4">
        {/* marca */}
        <div className="flex items-center gap-2.5">
          <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-white/15 ring-1 ring-white/25">
            <GraduationCap className="h-5 w-5" />
          </div>
          <div className="leading-tight">
            <p className="text-sm font-semibold tracking-tight">Controle de Disciplinas</p>
            <p className="text-[11px] text-white/70">Eng. da Computação — UCP</p>
          </div>
        </div>

        {/* navegação desktop */}
        <nav className="hidden items-center gap-1 md:flex">
          {itens.map((item) => (
            <LinkNav key={item.to} to={item.to} Icone={item.icone} rotulo={item.rotulo} />
          ))}
        </nav>

        {/* usuário + sair (desktop) */}
        <div className="hidden items-center gap-3 md:flex">
          <div className="flex items-center gap-2">
            <UserAvatar />
            <span className="max-w-[10rem] truncate text-sm font-medium">{aluno?.nome}</span>
          </div>
          <Button
            variant="ghost"
            size="sm"
            onClick={logout}
            className="text-white hover:bg-white/15 hover:text-white"
          >
            <LogOut className="h-4 w-4" />
            <span>Sair</span>
          </Button>
        </div>

        {/* botão de menu (mobile) */}
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

      {/* navegação mobile (dropdown) */}
      {menuAberto && (
        <div className="border-t border-white/15 bg-teal-700/95 px-4 pb-4 pt-2 md:hidden">
          <nav className="flex flex-col gap-1">
            {itens.map((item) => (
              <LinkNav
                key={item.to}
                to={item.to}
                Icone={item.icone}
                rotulo={item.rotulo}
                onClick={fecharMenu}
              />
            ))}
          </nav>
          <div className="mt-3 flex items-center justify-between border-t border-white/15 pt-3">
            <div className="flex items-center gap-2">
              <UserAvatar />
              <span className="text-sm font-medium">{aluno?.nome}</span>
            </div>
            <Button
              variant="ghost"
              size="sm"
              onClick={logout}
              className="text-white hover:bg-white/15 hover:text-white"
            >
              <LogOut className="h-4 w-4" />
              Sair
            </Button>
          </div>
        </div>
      )}
    </header>
  );
}
