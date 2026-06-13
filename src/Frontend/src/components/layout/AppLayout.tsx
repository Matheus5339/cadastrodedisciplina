import { Outlet } from "react-router-dom";
import { TopNav } from "@/components/layout/TopNav";

/** Layout autenticado: topbar horizontal + conteúdo centralizado. */
export function AppLayout() {
  return (
    <div className="flex min-h-screen flex-col">
      <TopNav />
      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
        <Outlet />
      </main>
    </div>
  );
}
