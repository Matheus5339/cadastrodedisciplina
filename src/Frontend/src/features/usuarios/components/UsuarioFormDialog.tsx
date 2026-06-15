import { useState, type FormEvent } from "react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Dialog } from "@/components/ui/dialog";
import { FormField } from "@/components/forms/FormField";
import { usuariosApi } from "@/features/usuarios/services/usuarios-api";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { notificarSucesso } from "@/services/notification";
import type { Perfil, UsuarioDto } from "@/types/api";

interface Props {
  usuario: UsuarioDto | null; // null = novo
  onFechar: () => void;
  onSalvo: () => void;
}

export function UsuarioFormDialog({ usuario, onFechar, onSalvo }: Props) {
  const editando = usuario !== null;
  const [login, setLogin] = useState(usuario?.login ?? "");
  const [senha, setSenha] = useState("");
  const [perfil, setPerfil] = useState<Perfil>(usuario?.perfil ?? "Colecionador");
  const [erro, setErro] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);

  async function salvar(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setSalvando(true);
    try {
      if (editando) await usuariosApi.atualizar(usuario.id, login, perfil);
      else await usuariosApi.criar(login, senha, perfil);
      notificarSucesso(editando ? "Usuário atualizado." : "Usuário criado.");
      onSalvo();
    } catch (err) {
      setErro(obterMensagemDeErro(err));
    } finally {
      setSalvando(false);
    }
  }

  return (
    <Dialog aberto onFechar={onFechar} titulo={editando ? "Editar usuário" : "Novo usuário"}>
      <form onSubmit={salvar} className="space-y-4" noValidate>
        {erro && <Alert variant="destructive">{erro}</Alert>}

        <FormField id="login" label="Login">
          <Input id="login" value={login} onChange={(e) => setLogin(e.target.value)} required minLength={3} maxLength={50} />
        </FormField>

        {!editando && (
          <FormField id="senha" label="Senha">
            <Input
              id="senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              required
              minLength={8}
            />
          </FormField>
        )}

        <FormField id="perfil" label="Perfil">
          <Select id="perfil" value={perfil} onChange={(e) => setPerfil(e.target.value as Perfil)}>
            <option value="Administrador">Administrador</option>
            <option value="Autor">Autor</option>
            <option value="Colecionador">Colecionador</option>
          </Select>
        </FormField>

        <div className="flex justify-end gap-2 pt-2">
          <Button type="button" variant="outline" onClick={onFechar}>
            Cancelar
          </Button>
          <Button type="submit" disabled={salvando}>
            {salvando ? "Salvando..." : "Ok"}
          </Button>
        </div>
      </form>
    </Dialog>
  );
}
