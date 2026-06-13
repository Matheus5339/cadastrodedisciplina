import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { ANO_MAX, ANO_MIN, SEMESTRES } from "@/config/constants";

export interface FiltrosState {
  nome: string;
  professor: string;
  ano: string;
  semestre: string;
}

interface DisciplinaFiltersProps {
  filtros: FiltrosState;
  onChange: (filtros: FiltrosState) => void;
}

/** Filtros por nome, professor, semestre e/ou ano (requisito do normativo §2). */
export function DisciplinaFilters({ filtros, onChange }: DisciplinaFiltersProps) {
  const anoAtual = new Date().getFullYear();
  const anos = Array.from({ length: anoAtual - 2015 + 1 }, (_, i) => anoAtual - i).filter(
    (a) => a >= ANO_MIN && a <= ANO_MAX,
  );

  return (
    <div className="grid gap-3 rounded-lg border border-border bg-card p-4 sm:grid-cols-2 lg:grid-cols-4">
      <div className="space-y-1.5">
        <Label htmlFor="filtro-nome">Nome ou código</Label>
        <div className="relative">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            id="filtro-nome"
            className="pl-8"
            placeholder="Buscar disciplina..."
            value={filtros.nome}
            onChange={(e) => onChange({ ...filtros, nome: e.target.value })}
          />
        </div>
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="filtro-professor">Professor</Label>
        <Input
          id="filtro-professor"
          placeholder="Nome do professor"
          value={filtros.professor}
          onChange={(e) => onChange({ ...filtros, professor: e.target.value })}
        />
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="filtro-ano">Ano cursado</Label>
        <Select
          id="filtro-ano"
          value={filtros.ano}
          onChange={(e) => onChange({ ...filtros, ano: e.target.value })}
        >
          <option value="">Todos</option>
          {anos.map((ano) => (
            <option key={ano} value={ano}>
              {ano}
            </option>
          ))}
        </Select>
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="filtro-semestre">Semestre cursado</Label>
        <Select
          id="filtro-semestre"
          value={filtros.semestre}
          onChange={(e) => onChange({ ...filtros, semestre: e.target.value })}
        >
          <option value="">Todos</option>
          {SEMESTRES.map((s) => (
            <option key={s} value={s}>
              {s}º semestre
            </option>
          ))}
        </Select>
      </div>
    </div>
  );
}
