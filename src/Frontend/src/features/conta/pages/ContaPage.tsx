import { useState, type FormEvent } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormField } from "@/components/forms/FormField";
import { useAuthStore } from "@/core/auth/auth-store";
import { useSessao } from "@/core/session/useSessao";
import { contaApi } from "@/features/conta/services/conta-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarErro, notificarSucesso } from "@/services/notification";

export function ContaPage() {
  const { usuario } = useSessao();
  const atualizarUsuario = useAuthStore((s) => s.atualizarUsuario);
  const [login, setLogin] = useState(usuario?.login ?? "");
  const [novaSenha, setNovaSenha] = useState("");

  async function salvarLogin(e: FormEvent) {
    e.preventDefault();
    try {
      const atualizado = await contaApi.atualizarLogin(login);
      atualizarUsuario(atualizado);
      notificarSucesso("Login atualizado.");
    } catch (err) {
      notificarErro(obterMensagemDeErro(err));
    }
  }

  async function salvarSenha(e: FormEvent) {
    e.preventDefault();
    try {
      await contaApi.trocarSenha(novaSenha);
      setNovaSenha("");
      notificarSucesso("Senha alterada com sucesso.");
    } catch (err) {
      notificarErro(obterMensagemDeErro(err));
    }
  }

  return (
    <div className="mx-auto max-w-xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Minha conta</h1>
        <p className="text-sm text-muted-foreground">Perfil: {usuario?.perfil}</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Login</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={salvarLogin} className="space-y-3">
            <FormField id="login" label="Login">
              <Input id="login" value={login} onChange={(e) => setLogin(e.target.value)} minLength={3} maxLength={50} required />
            </FormField>
            <Button type="submit" size="sm">Salvar login</Button>
          </form>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Trocar senha</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={salvarSenha} className="space-y-3">
            <FormField id="novaSenha" label="Nova senha">
              <Input id="novaSenha" type="password" value={novaSenha} onChange={(e) => setNovaSenha(e.target.value)} minLength={8} required />
            </FormField>
            <Button type="submit" size="sm">Alterar senha</Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
