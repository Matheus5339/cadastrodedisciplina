import { create } from "zustand";

interface UiState {
  sidebarAberta: boolean;
  alternarSidebar: () => void;
  fecharSidebar: () => void;
}

/** Estado global de UI (sidebar responsiva). */
export const useUiStore = create<UiState>((set) => ({
  sidebarAberta: false,
  alternarSidebar: () => set((s) => ({ sidebarAberta: !s.sidebarAberta })),
  fecharSidebar: () => set({ sidebarAberta: false }),
}));
