/** Tipos espelhando os DTOs da API (álbum de figurinhas). */

export type Perfil = "Administrador" | "Autor" | "Colecionador";

export interface UsuarioDto {
  id: number;
  login: string;
  perfil: Perfil;
}

export interface AuthResultDto {
  accessToken: string;
  accessTokenExpiresAtUtc: string;
  usuario: UsuarioDto;
  // o refresh token viaja em cookie httpOnly
}

export interface AlbumDto {
  id: number;
  nome: string;
  paginas: number;
  possuiCapa: boolean;
}

export interface FigurinhaDto {
  id: number;
  numero: number;
  nome: string;
  pagina: number;
  descricao: string | null;
  tag: string;
  possuiImagem: boolean;
}

export interface FigurinhaAlbumDto {
  id: number;
  numero: number;
  nome: string;
  pagina: number;
  tag: string;
  possuiImagem: boolean;
  adquirida: boolean;
}

export interface AlbumColecionadorDto {
  album: AlbumDto;
  figurinhas: FigurinhaAlbumDto[];
}

export interface ApiErrorBody {
  status: number;
  titulo: string;
  detalhe: string;
  traceId?: string;
}
