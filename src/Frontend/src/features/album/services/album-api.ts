import { http } from "@/core/http/client";
import type { AlbumDto, FigurinhaDto } from "@/types/api";

export const albumApi = {
  obter: () => http.get<AlbumDto>("/album").then((r) => r.data),
  atualizar: (nome: string, paginas: number) =>
    http.put<AlbumDto>("/album", { nome, paginas }).then((r) => r.data),
  enviarCapa: (arquivo: File) => {
    const fd = new FormData();
    fd.append("imagem", arquivo);
    return http.put("/album/capa", fd);
  },
  capaUrl: "/album/capa",
};

export const figurinhasApi = {
  listar: (texto?: string, pagina?: number) =>
    http
      .get<FigurinhaDto[]>("/figurinhas", {
        params: { texto: texto || undefined, pagina: pagina || undefined },
      })
      .then((r) => r.data),
  obter: (id: number) => http.get<FigurinhaDto>(`/figurinhas/${id}`).then((r) => r.data),
  criar: (dados: FormData) => http.post<FigurinhaDto>("/figurinhas", dados).then((r) => r.data),
  atualizar: (id: number, dados: FormData) =>
    http.put<FigurinhaDto>(`/figurinhas/${id}`, dados).then((r) => r.data),
  remover: (id: number) => http.delete(`/figurinhas/${id}`),
  limpar: () => http.post("/figurinhas/limpar"),
  imagemUrl: (id: number) => `/figurinhas/${id}/imagem`,
};

export const arquivosApi = {
  exportarTexto: () =>
    http.get("/arquivos/figurinhas/texto", { responseType: "blob" }).then((r) => r.data as Blob),
  exportarBinario: () =>
    http.get("/arquivos/figurinhas/binario", { responseType: "blob" }).then((r) => r.data as Blob),
  importarBinario: (arquivo: File) => {
    const fd = new FormData();
    fd.append("arquivo", arquivo);
    return http.post<{ importadas: number }>("/arquivos/figurinhas/binario", fd).then((r) => r.data);
  },
};
