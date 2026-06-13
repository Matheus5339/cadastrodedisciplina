import { create } from "zustand";

export interface Notificacao {
  id: number;
  tipo: "sucesso" | "erro";
  mensagem: string;
}

interface NotificationState {
  notificacoes: Notificacao[];
  notificar: (tipo: Notificacao["tipo"], mensagem: string) => void;
  remover: (id: number) => void;
}

let proximoId = 1;

export const useNotificationStore = create<NotificationState>((set) => ({
  notificacoes: [],
  notificar: (tipo, mensagem) => {
    const id = proximoId++;
    set((s) => ({ notificacoes: [...s.notificacoes, { id, tipo, mensagem }] }));
    setTimeout(() => {
      set((s) => ({ notificacoes: s.notificacoes.filter((n) => n.id !== id) }));
    }, 5000);
  },
  remover: (id) => set((s) => ({ notificacoes: s.notificacoes.filter((n) => n.id !== id) })),
}));

export const notificarSucesso = (mensagem: string) =>
  useNotificationStore.getState().notificar("sucesso", mensagem);

export const notificarErro = (mensagem: string) =>
  useNotificationStore.getState().notificar("erro", mensagem);
