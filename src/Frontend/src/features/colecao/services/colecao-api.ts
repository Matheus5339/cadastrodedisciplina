import { http } from "@/core/http/client";
import type { AlbumColecionadorDto, FigurinhaDto } from "@/types/api";

export const colecaoApi = {
  meuAlbum: () => http.get<AlbumColecionadorDto>("/colecao/album").then((r) => r.data),
  obterFigurinha: (id: number) => http.get<FigurinhaDto>(`/figurinhas/${id}`).then((r) => r.data),
  consultar: (tag: string) =>
    http.get<FigurinhaDto>(`/colecao/consultar/${encodeURIComponent(tag)}`).then((r) => r.data),
  adquirir: (tag: string) => http.post<FigurinhaDto>("/colecao/adquirir", { tag }).then((r) => r.data),
  imagemUrl: (id: number) => `/figurinhas/${id}/imagem`,
};
