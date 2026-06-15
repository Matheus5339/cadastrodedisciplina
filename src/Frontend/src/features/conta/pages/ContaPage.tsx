import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { FormField } from "@/components/forms/FormField";
import { useAuthStore } from "@/core/auth/auth-store";
import { useSessao } from "@/core/session/useSessao";
import { contaApi } from "@/features/conta/services/conta-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarSucesso } from "@/services/notification";
import { rotaInicial } from "@/routes/paths";

/**
 * FrmUsuario na visão do próprio usuário (PDF §7): troca o próprio login e senha,
 * mas NÃO o perfil — que aparece desabilitado.
 */
export function ContaPage() {
  const navigate = useNavigate();
  const { usuario } = useSessao();
  const atualizarUsuario = useAuthStore((s) => s.atualizarUsuario);

  const loginOriginal = usuario?.login ?? "";
  const [login, setLogin] = useState(loginOriginal);
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);

  async function salvar(e: FormEvent) {
    e.preventDefault();
    setErro(null);

    if (!login.trim() || login.trim().length < 3) {
      setErro("O login deve ter ao menos 3 caracteres.");
      return;
    }
    if (senha && !/^(?=.*[A-Za-z])(?=.*\d).{8,128}$/.test(senha)) {
      setErro("A nova senha deve ter de 8 a 128 caracteres, com pelo menos uma letra e um número.");
      return;
    }

    setSalvando(true);
    try {
      let mudou = false;
      if (login.trim() !== loginOriginal) {
        const atualizado = await contaApi.atualizarLogin(login.trim());
        atualizarUsuario(atualizado);
        mudou = true;
      }
      if (senha) {
        await contaApi.trocarSenha(senha);
        setSenha("");
        mudou = true;
      }
      notificarSucesso(mudou ? "Dados atualizados." : "Nenhuma alteração a salvar.");
    } catch (err) {
      setErro(obterMensagemDeErro(err));
    } finally {
      setSalvando(false);
    }
  }

  function cancelar() {
    setLogin(loginOriginal);
    setSenha("");
    setErro(null);
    if (usuario) navigate(rotaInicial(usuario.perfil));
  }

  return (
    <div className="mx-auto max-w-md">
      <div className="rounded-xl border border-border bg-card shadow-sm">
        <div className="border-b border-border px-4 py-2.5">
          <h1 className="text-base font-semibold">Minha conta</h1>
        </div>

        <form onSubmit={salvar} className="space-y-4 p-4" noValidate>
          {erro && <Alert variant="destructive">{erro}</Alert>}

          <FormField id="login" label="Login">
            <Input id="login" value={login} onChange={(e) => setLogin(e.target.value)} minLength={3} maxLength={50} required />
          </FormField>

          <FormField id="senha" label="Senha">
            <Input
              id="senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              placeholder="nova senha (deixe em branco para manter)"
              minLength={8}
            />
            <p className="mt-1 text-xs text-muted-foreground">
              Para alterar: mínimo 8 caracteres, com pelo menos uma letra e um número.
            </p>
          </FormField>

          {/* Perfil DESABILITADO (PDF §7): o usuário não altera o próprio perfil */}
          <FormField id="perfil" label="Perfil">
            <Select id="perfil" value={usuario?.perfil ?? "Colecionador"} disabled>
              <option value="Administrador">Administrador</option>
              <option value="Autor">Autor</option>
              <option value="Colecionador">Colecionador</option>
            </Select>
          </FormField>

          <div className="flex justify-end gap-2 pt-1">
            <Button type="button" variant="outline" onClick={cancelar}>
              Cancelar
            </Button>
            <Button type="submit" disabled={salvando}>
              {salvando ? "Salvando..." : "Ok"}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
