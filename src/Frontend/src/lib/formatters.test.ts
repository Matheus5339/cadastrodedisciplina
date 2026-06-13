import { describe, it, expect } from "vitest";
import { formatarCr, formatarCpf } from "@/lib/formatters";

describe("formatarCr", () => {
  it("formata com 2 casas no padrão brasileiro", () => {
    expect(formatarCr(8.5)).toBe("8,50");
    expect(formatarCr(10)).toBe("10,00");
  });

  it("retorna travessão quando não há CR", () => {
    expect(formatarCr(null)).toBe("—");
    expect(formatarCr(undefined)).toBe("—");
  });
});

describe("formatarCpf", () => {
  it("aplica a máscara 000.000.000-00", () => {
    expect(formatarCpf("12345678901")).toBe("123.456.789-01");
  });

  it("ignora caracteres não numéricos e limita a 11 dígitos", () => {
    expect(formatarCpf("123.456.789-01extra")).toBe("123.456.789-01");
  });
});
