import { useEffect, useState } from "react";
import { useSessao } from "@/core/session/useSessao";
import { alunoApi } from "@/features/profile/services/aluno-api";
import { cn } from "@/lib/utils";

/**
 * Foto do aluno (carregada autenticada, via blob) com fallback para iniciais.
 * Recarrega quando fotoVersao muda (após novo upload).
 */
export function UserAvatar({ className }: { className?: string }) {
  const { aluno, fotoVersao } = useSessao();
  const [url, setUrl] = useState<string | null>(null);

  useEffect(() => {
    let revogada = false;
    let objectUrl: string | null = null;

    if (aluno?.possuiFoto || fotoVersao > 0) {
      alunoApi
        .obterFotoBlob()
        .then((blob) => {
          objectUrl = URL.createObjectURL(blob);
          if (!revogada) setUrl(objectUrl);
        })
        .catch(() => setUrl(null));
    } else {
      setUrl(null);
    }

    return () => {
      revogada = true;
      if (objectUrl) URL.revokeObjectURL(objectUrl);
    };
  }, [aluno?.possuiFoto, fotoVersao]);

  const iniciais = (aluno?.nome ?? "?")
    .split(/\s+/)
    .slice(0, 2)
    .map((p) => p[0]?.toUpperCase() ?? "")
    .join("");

  return url ? (
    <img
      src={url}
      alt={`Foto de ${aluno?.nome ?? "aluno"}`}
      className={cn("h-8 w-8 rounded-full object-cover ring-1 ring-border", className)}
    />
  ) : (
    <div
      className={cn(
        "flex h-8 w-8 items-center justify-center rounded-full bg-primary/15 text-xs font-semibold text-primary",
        className,
      )}
      aria-hidden
    >
      {iniciais}
    </div>
  );
}
