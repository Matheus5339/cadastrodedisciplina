import type { ReactNode } from "react";
import { Toaster } from "@/components/feedback/Toaster";

/**
 * Composição de providers globais. Estado global usa Zustand (sem Context),
 * então aqui ficam apenas elementos globais de UI (notificações).
 */
export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <>
      {children}
      <Toaster />
    </>
  );
}
