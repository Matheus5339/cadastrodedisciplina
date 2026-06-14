import { useEffect, useState } from "react";
import { ImageOff } from "lucide-react";
import { http } from "@/core/http/client";
import { cn } from "@/lib/utils";

/**
 * Exibe uma imagem servida pela API (que exige Bearer): busca o blob pelo
 * cliente HTTP autenticado e gera um object URL. Mostra fallback se não houver.
 */
export function ImagemAuth({
  src,
  alt,
  className,
  carregar = true,
}: {
  src: string;
  alt: string;
  className?: string;
  carregar?: boolean;
}) {
  const [url, setUrl] = useState<string | null>(null);
  const [falhou, setFalhou] = useState(false);

  useEffect(() => {
    if (!carregar) return;
    let ativo = true;
    let objectUrl: string | null = null;
    setFalhou(false);

    http
      .get(src, { responseType: "blob" })
      .then((resp) => {
        objectUrl = URL.createObjectURL(resp.data as Blob);
        if (ativo) setUrl(objectUrl);
      })
      .catch(() => ativo && setFalhou(true));

    return () => {
      ativo = false;
      if (objectUrl) URL.revokeObjectURL(objectUrl);
    };
  }, [src, carregar]);

  if (falhou || !carregar) {
    return (
      <div className={cn("flex items-center justify-center bg-muted text-muted-foreground", className)}>
        <ImageOff className="h-6 w-6" />
      </div>
    );
  }

  if (!url) {
    return <div className={cn("animate-pulse bg-muted", className)} />;
  }

  return <img src={url} alt={alt} className={cn("object-cover", className)} />;
}
