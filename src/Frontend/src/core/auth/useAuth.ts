import { useNavigate } from "react-router-dom";
import { useAuthStore } from "@/core/auth/auth-store";
import { authApi } from "@/features/auth/services/auth-api";
import { secureStorage } from "@/core/security/storage";

/** Ações de autenticação compartilhadas (logout com invalidação no servidor — segurança 16). */
export function useAuth() {
  const navigate = useNavigate();
  const { limparSessao } = useAuthStore();

  async function logout() {
    try {
      await authApi.logout();
    } catch {
      // mesmo se a API falhar, a sessão local é encerrada
    } finally {
      limparSessao();
      secureStorage.clear();
      navigate("/login");
    }
  }

  return { logout };
}
