import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { EmptyState } from "@/components/feedback/EmptyState";

describe("EmptyState", () => {
  it("mostra o título e a descrição", () => {
    render(<EmptyState titulo="Nenhuma disciplina" descricao="Cadastre a primeira" />);
    expect(screen.getByText("Nenhuma disciplina")).toBeInTheDocument();
    expect(screen.getByText("Cadastre a primeira")).toBeInTheDocument();
  });

  it("não renderiza a descrição quando ausente", () => {
    render(<EmptyState titulo="Lista vazia" />);
    expect(screen.getByText("Lista vazia")).toBeInTheDocument();
    expect(screen.queryByText("Cadastre a primeira")).not.toBeInTheDocument();
  });
});
