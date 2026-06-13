import { LogOut, Menu } from "lucide-react";
import { Button } from "@/components/ui/button";
import { UserAvatar } from "@/components/layout/UserAvatar";
import { useAuth } from "@/core/auth/useAuth";
import { useSessao } from "@/core/session/useSessao";
import { useUiStore } from "@/stores/ui-store";

export function Header() {
  const { aluno } = useSessao();
  const { logout } = useAuth();
  const alternarSidebar = useUiStore((s) => s.alternarSidebar);

  return (
    <header className="flex h-14 items-center justify-between border-b border-border bg-card px-4">
      <Button variant="ghost" size="icon" className="md:hidden" onClick={alternarSidebar} aria-label="Abrir menu">
        <Menu className="h-5 w-5" />
      </Button>
      <div className="hidden md:block" />
      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2">
          <UserAvatar />
          <span className="hidden text-sm font-medium sm:inline">{aluno?.nome}</span>
        </div>
        <Button variant="ghost" size="sm" onClick={logout}>
          <LogOut className="h-4 w-4" />
          <span className="hidden sm:inline">Sair</span>
        </Button>
      </div>
    </header>
  );
}
