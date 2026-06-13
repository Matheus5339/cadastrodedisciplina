// Estende o expect do Vitest com os matchers do jest-dom (toBeInTheDocument etc.).
import "@testing-library/jest-dom/vitest";
import { afterEach } from "vitest";
import { cleanup } from "@testing-library/react";

// Com globals desativados, o auto-cleanup do Testing Library não é registrado:
// limpamos o DOM manualmente entre os testes para evitar vazamento de estado.
afterEach(() => {
  cleanup();
});
