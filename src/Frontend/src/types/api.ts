/** Tipos espelhando os DTOs da API. */

export interface AlunoDto {
  id: number;
  rgu: string;
  cpf: string;
  email: string;
  nome: string;
  possuiFoto: boolean;
}

export interface AuthResultDto {
  accessToken: string;
  accessTokenExpiresAtUtc: string;
  aluno: AlunoDto;
  // O refresh token não vem no corpo: viaja em cookie httpOnly gerenciado pelo navegador.
}

export interface DisciplinaDto {
  id: number;
  codigo: string;
  nome: string;
  professor: string | null;
  periodo: number;
  creditos: number;
}

export interface HistoricoDto {
  id: number;
  disciplinaId: number;
  disciplinaCodigo: string;
  disciplinaNome: string;
  disciplinaProfessor: string | null;
  creditos: number;
  ano: number;
  semestre: number;
  periodo: number;
  mediaFinal: number;
}

export interface CrDto {
  cr: number | null;
  totalCreditos: number;
  totalDisciplinas: number;
}

export interface ApiErrorBody {
  status: number;
  titulo: string;
  detalhe: string;
  traceId?: string;
}
