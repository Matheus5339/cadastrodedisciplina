import { BookOpen, GraduationCap, LayoutDashboard, ScrollText, User } from "lucide-react";
import { NavItem } from "@/components/navigation/NavItem";
import { useUiStore } from "@/stores/ui-store";
import { cn } from "@/lib/utils";
import { paths } from "@/routes/paths";

const itens = [
  { to: paths.dashboard, icone: <LayoutDashboard className="h-4 w-4" />, rotulo: "Painel" },
  { to: paths.disciplinas, icone: <BookOpen className="h-4 w-4" />, rotulo: "Disciplinas" },
  { to: paths.historico, icone: <ScrollText className="h-4 w-4" />, rotulo: "Histórico" },
  { to: paths.perfil, icone: <User className="h-4 w-4" />, rotulo: "Perfil" },
];

export function Sidebar() {
  const { sidebarAberta, fecharSidebar } = useUiStore();

  return (
    <>
      {/* backdrop no mobile */}
      {sidebarAberta && (
        <div className="fixed inset-0 z-30 bg-black/40 md:hidden" onClick={fecharSidebar} />
      )}
      <aside
        className={cn(
          "fixed inset-y-0 left-0 z-40 w-60 border-r border-border bg-card transition-transform md:static md:translate-x-0",
          sidebarAberta ? "translate-x-0" : "-translate-x-full",
        )}
      >
        <div className="flex h-14 items-center gap-2 border-b border-border px-4">
          <GraduationCap className="h-6 w-6 text-primary" />
          <div className="leading-tight">
            <p className="text-sm font-semibold">Controle de Disciplinas</p>
            <p className="text-xs text-muted-foreground">Eng. da Computação — UCP</p>
          </div>
        </div>
        <nav className="space-y-1 p-3">
          {itens.map((item) => (
            <NavItem key={item.to} to={item.to} icone={item.icone} onClick={fecharSidebar}>
              {item.rotulo}
            </NavItem>
          ))}
        </nav>
      </aside>
    </>
  );
}
