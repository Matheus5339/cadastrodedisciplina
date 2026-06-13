#!/usr/bin/env python3
"""
Gera src/Backend/Api/data/disciplinas.csv combinando:
  - Engenharia de Computação (CSV existente, copiado verbatim)
  - 6 cursos do CEC extraídos das matrizes públicas da UCP (ucp.br)

Campos: codigo,nome,professor,periodo,creditos
Regras:
  - codigo: PREFIXO-NNN sequencial por curso
  - professor: vazio (a UCP não publica o docente por disciplina)
  - creditos: carga horária / 18 (1 crédito = 18h, padrão do CSV de Computação)
  - periodo: número do período da matriz
Fontes: páginas oficiais de cada curso em https://ucp.br/<curso>/
"""
import csv
import os
import re

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.dirname(AQUI)
ECOMP_EXTERNO = os.path.abspath(os.path.join(RAIZ, "..", "disciplinas-ecomp-ucp.csv"))
SAIDA = os.path.join(RAIZ, "src", "Backend", "Api", "data", "disciplinas.csv")

HORAS_POR_CREDITO = 18

# período | nome | horas  (texto verbatim das matrizes da UCP)
CURSOS = {
    "CIV": ("Engenharia Civil", """
1 | Linguagem e Redação I | 72
1 | Segurança no Trabalho | 36
1 | Administração e Organização Empresarial | 36
1 | Química | 72
1 | Ciência dos Materiais | 36
1 | Álgebra Linear | 36
1 | Humanismo Cristão e Fenômeno Religioso | 72
1 | Introdução à Inteligência Artificial | 36
1 | Introdução à Engenharia | 36
1 | Introdução à Computação | 72
1 | Introdução ao Cálculo | 72
1 | Geometria Analítica | 36
2 | Tópicos de Teologia | 72
2 | Introdução à Física | 36
2 | Ergonomia e Desenho Universal | 36
2 | Projetos e Ativ. de Extensão I (Gerência) | 72
2 | Ciências do Ambiente | 36
2 | Cálculo I | 72
2 | Estatística | 72
2 | Programação I | 72
2 | Introdução à Filosofia | 36
2 | Mecânica Clássica | 72
3 | Projetos e Ativ. de Extensão II (IA) | 72
3 | Mecânica Geral I | 72
3 | Cálculo II | 72
3 | Ética | 36
3 | Programação II | 72
3 | Introdução à Teoria Econômica | 36
3 | Eletricidade e Magnetismo | 72
4 | Projetos e Ativ. de Extensão III (Inovação) | 72
4 | Resistência dos Materiais I | 72
4 | Ética Profissional em Engenharia | 36
4 | Análise Vetorial | 72
4 | Cálculo III | 72
4 | Fluidos, Ondas e Calor | 72
4 | Desenho Técnico de Construção Civil | 72
5 | Resistência dos Materiais II | 72
5 | Materiais de Construção Civil | 72
5 | Geologia Geral | 36
5 | Desenho Computacional na Const. Civil | 72
5 | Topografia | 72
5 | Laboratório de Mecânica dos Fluidos | 36
5 | Mecânica dos Fluidos | 36
6 | Análise Estrutural | 72
6 | Hidráulica | 72
6 | Estruturas de Concreto Armado I | 72
6 | Tecnologia da Construção Civil | 72
6 | Mecânica dos Solos I | 72
6 | Laboratório de Mecânica dos Solos | 36
6 | Obras de Terra | 36
7 | Projetos e Ativ. de Extensão IV | 72
7 | Hidrologia | 72
7 | Estruturas de Concreto Armado II | 72
7 | Mecânica dos Solos II | 72
7 | Instalações Elétricas | 72
7 | Instalações Hidráulicas | 72
7 | Laboratório de Materiais de Const. Civil | 36
8 | Projetos e Ativ. de Extensão V | 72
8 | Pontes | 72
8 | Saneamento | 72
8 | Programação e Controle de Obras | 72
8 | Estruturas de Aço | 72
8 | Fundações | 72
8 | Metodologia Científica no TCC | 36
9 | Projetos e Ativ. de Extensão VI | 72
9 | Arquitetura e Urbanismo | 36
9 | Estruturas de Madeira | 36
9 | Estágio Supervisionado | 198
10 | Trabalho de Conclusão de Curso | 18
10 | Transportes e Construção de Estradas | 72
"""),
    "PROD": ("Engenharia de Produção", """
1 | Linguagem e Redação I | 72
1 | Segurança no Trabalho | 36
1 | Administração e Organização Empresarial | 36
1 | Química | 72
1 | Ciência dos Materiais | 36
1 | Álgebra Linear | 36
1 | Humanismo Cristão e Fenômeno Religioso | 72
1 | Introdução à Inteligência Artificial | 36
1 | Introdução à Engenharia | 36
1 | Introdução à Computação | 72
2 | Introdução ao Cálculo | 72
2 | Geometria Analítica | 36
2 | Introdução à Teoria Econômica | 36
2 | Tópicos de Teologia | 72
2 | Introdução à Física | 36
2 | Ergonomia e Desenho Universal | 36
2 | Projetos e Ativ. de Extensão I (Gerência) | 72
2 | Ciências do Ambiente | 36
3 | Cálculo I | 72
3 | Estatística | 72
3 | Programação I | 72
3 | Introdução à Filosofia | 36
3 | Mecânica Clássica | 72
3 | Projetos e Ativ. de Extensão II (IA) | 72
4 | Cálculo II | 72
4 | Ética | 36
4 | Programação II | 72
4 | Eletricidade e Magnetismo | 72
4 | Mecânica dos Sólidos | 36
4 | Projetos e Ativ. de Extensão III | 72
5 | Gerência de Operações | 90
5 | Ética Profissional em Engenharia | 36
5 | Análise Vetorial | 72
5 | Desenho Técnico I | 72
5 | Cálculo III | 72
5 | Eletrotécnica | 72
5 | Programação Estruturada | 36
5 | Fluidos, Ondas e Calor | 72
6 | Arranjo Físico Industrial | 36
6 | Desenho Técnico II | 72
6 | Fenômenos de Transporte | 36
6 | Lab. Processos Especiais de Fabricação | 36
6 | Materiais de Construção Mecânica | 36
6 | Lab. Materiais de Construção Mecânica | 36
6 | Projeto de Produtos | 36
6 | Controle Estatístico de Processos | 36
6 | Processos Especiais de Fabricação | 36
6 | Metrologia Industrial | 36
7 | Gestão da Qualidade | 54
7 | Lab. Tecnologia de Usinagem | 36
7 | Lean Manufacturing | 36
7 | Gestão de Estoques e Sistemas MRP | 36
7 | Engenharia de Confiabilidade | 36
7 | Tecnologia de Usinagem | 36
7 | Tecnologias de Inspeções e Testes | 72
7 | Manutenção de Equipamentos | 36
8 | Gestão Logística e Cadeia de Suprimentos | 54
8 | Projetos e Ativ. de Extensão IV | 72
8 | Programação e Controle da Produção | 54
8 | Processos Estocásticos | 36
8 | Teoria dos Jogos | 36
8 | Gestão de Sistemas de Informação | 36
8 | Inferência Estatística | 36
9 | Inteligência Artificial | 72
9 | Programação em Python | 36
9 | Projetos e Ativ. de Extensão V | 72
9 | Pesquisa Operacional | 90
9 | Técnicas de Simulação | 36
9 | Simulação de Negócios (Business Game) | 36
9 | Contabilidade Introdutória | 36
9 | Gestão de Custos e Preços | 36
9 | Análise de Dados | 36
9 | Metodologia Científica e Tecnológica | 36
10 | Matemática Financeira | 54
10 | Engenharia Econômica | 36
10 | Gestão Financeira e Orçamento | 36
10 | Mercado Financeiro e de Capitais | 108
10 | Estratégia de Negócios | 36
10 | Trabalho de Conclusão de Curso | 18
10 | Estágio Supervisionado | 198
10 | Projetos e Ativ. de Extensão VI | 72
"""),
    "ELET": ("Engenharia Elétrica", """
1 | Linguagem e Redação I | 72
1 | Introdução ao Cálculo | 72
1 | Introdução à Computação | 72
1 | Introdução à Física | 36
1 | Humanismo Cristão e Fenômeno Religioso | 72
1 | Ciências do Ambiente | 36
1 | Projetos e Ativ. de Extensão I | 72
2 | Cálculo I | 72
2 | Programação I | 72
2 | Estatística | 72
2 | Geometria Analítica | 36
2 | Introdução à Filosofia | 36
2 | Segurança no Trabalho | 36
2 | Projetos e Ativ. de Extensão II | 72
3 | Cálculo II | 72
3 | Programação II | 72
3 | Sinais e Sistemas | 72
3 | Mecânica Clássica | 72
3 | Tópicos de Teologia | 72
3 | Ética | 36
3 | Projetos e Ativ. de Extensão III | 72
4 | Análise Vetorial | 72
4 | Cálculo III | 72
4 | Programação Estruturada | 36
4 | Eletricidade e Magnetismo | 72
5 | Fluidos, Ondas e Calor | 72
5 | Circuitos Elétricos I | 72
5 | Processamento Digital de Sinais | 72
5 | Técnicas Digitais | 72
5 | Álgebra Linear | 36
5 | Introdução à Inteligência Artificial | 36
6 | Eletromagnetismo | 72
6 | Circuitos Elétricos II | 72
6 | Eletrônica I | 72
7 | Eletricidade Aplicada | 72
7 | Fenômenos de Transporte | 36
7 | Geração de Energia Elétrica | 36
7 | Mecânica dos Sólidos | 36
7 | Ética Profissional em Engenharia | 36
7 | Administração e Organização Empresarial | 36
8 | Conversão de Energia I | 72
8 | Eletrônica II | 72
8 | Controle e Servomecanismos I | 72
8 | Instalações Elétricas | 72
8 | Desenho de Instalações em Redes | 36
8 | Ciência dos Materiais | 36
8 | Projetos e Ativ. de Extensão IV | 72
9 | Conversão de Energia II | 72
9 | Controle e Servomecanismos II | 72
9 | Eletrônica Industrial | 72
9 | Automação de Processos Industriais | 72
9 | Laboratório de Controle | 36
9 | Fontes Alternativas de Energia | 36
9 | Inteligência Artificial | 72
9 | Projetos e Ativ. de Extensão V | 72
10 | Distribuição e Transmissão de Energia | 72
10 | Análise de Sistemas de Energia | 72
10 | Sistemas Fotovoltaicos | 72
10 | Sistemas de Geração Eólica e Hidráulica | 72
10 | Subestações Transformadoras | 36
10 | Planejamento Energético | 36
10 | Eficiência Energética | 36
10 | Metodologia Científica e Tecnológica | 36
10 | Trabalho de Conclusão de Curso | 18
10 | Estágio Supervisionado | 198
10 | Química | 72
10 | Introdução à Engenharia | 36
10 | Ergonomia e Desenho Universal | 36
10 | Introdução à Teoria Econômica | 36
10 | Projetos e Ativ. de Extensão VI | 72
"""),
    "MEC": ("Engenharia Mecânica", """
1 | Linguagem e Redação I | 72
1 | Introdução ao Cálculo | 72
1 | Geometria Analítica | 36
1 | Humanismo Cristão e Fenômeno Religioso | 72
1 | Introdução à Engenharia | 36
1 | Introdução à Computação | 72
2 | Química | 72
2 | Ciência dos Materiais | 36
2 | Álgebra Linear | 36
2 | Introdução à Inteligência Artificial | 36
2 | Introdução à Física | 36
3 | Cálculo I | 72
3 | Estatística | 72
3 | Programação I | 72
3 | Introdução à Filosofia | 36
3 | Mecânica Clássica | 72
4 | Mecânica Geral I | 72
4 | Cálculo II | 72
4 | Ética | 36
4 | Programação II | 72
4 | Introdução à Teoria Econômica | 36
4 | Eletricidade e Magnetismo | 72
5 | Resistência dos Materiais I | 72
5 | Ética Profissional em Engenharia | 36
5 | Análise Vetorial | 72
5 | Desenho Técnico I | 72
5 | Cálculo III | 72
5 | Eletrotécnica | 72
5 | Fluidos, Ondas e Calor | 72
6 | Mecânica Geral II | 72
6 | Resistência dos Materiais II | 72
6 | Termodinâmica Aplicada | 72
6 | Desenho Técnico II | 72
6 | Lab. de Mecânica dos Fluidos | 36
6 | Materiais de Construção Mecânica | 36
6 | Lab. de Materiais de Const. Mecânica | 36
6 | Mecânica dos Fluidos | 36
6 | Metrologia Industrial | 36
7 | Vibrações Mecânicas | 72
7 | Elementos de Máquinas I | 72
7 | Mecânica Aplicada | 72
7 | Lab. de Tecnologia de Usinagem | 36
7 | Tecnologia de Usinagem | 36
7 | Transferência de Calor e Massa | 36
7 | Desenho Técnico III | 36
8 | Elementos de Máquinas II | 72
8 | Máquinas Hidráulicas | 72
8 | Máquinas Térmicas | 72
8 | Lab. de Processos Especiais | 36
8 | Sistemas de Atuação | 72
8 | Elementos Finitos Aplicados | 36
8 | Processos Especiais de Fabricação | 36
9 | Tecnologias de Inspeções e Testes | 72
9 | Manutenção de Equipamentos | 36
9 | Automação de Processos Industriais | 72
9 | Refrigeração e Climatização | 72
9 | Metodologia Científica e Tecnológica | 36
9 | Introdução à Robótica | 72
9 | Controle Estatístico de Processos | 36
9 | Projeto de Instalações Industriais | 36
10 | Trabalho de Conclusão de Curso | 18
10 | Estágio Supervisionado | 198
10 | Introdução à Gestão da Produção | 36
3 | Projetos e Ativ. de Extensão I | 72
4 | Projetos e Ativ. de Extensão II (IA) | 72
5 | Projetos e Ativ. de Extensão III | 72
6 | Projetos e Ativ. de Extensão IV | 72
7 | Projetos e Ativ. de Extensão V | 72
8 | Projetos e Ativ. de Extensão VI | 72
"""),
    # Mecatrônica: a página repetiu o 10º período idêntico ao 9º (artefato);
    # mantidos apenas os períodos 1–9 efetivamente publicados.
    "MECT": ("Engenharia Mecatrônica", """
1 | Introdução à Engenharia | 36
1 | Química | 72
1 | Introdução à Computação | 72
1 | Introdução ao Cálculo | 72
1 | Introdução à Filosofia | 36
1 | Humanismo Cristão e Fenômeno Religioso | 72
1 | Introdução à Física | 36
2 | Cálculo I | 72
2 | Ética | 36
2 | Ciência dos Materiais | 36
2 | Programação I | 72
2 | Álgebra Linear | 36
2 | Geometria Analítica | 36
2 | Tópicos de Teologia | 72
2 | Mecânica Clássica | 72
3 | Mecânica Geral I | 72
3 | Cálculo II | 72
3 | Estatística | 72
3 | Programação II | 72
3 | Eletricidade e Magnetismo | 72
3 | Projetos e Atividades de Extensão I | 72
4 | Mecânica Geral II | 72
4 | Análise Vetorial | 72
4 | Cálculo III | 72
4 | Sinais e Sistemas | 72
4 | Fluidos, Ondas e Calor | 72
4 | Circuitos Elétricos I | 72
4 | Projetos e Atividades de Extensão II | 54
5 | Ciências do Ambiente | 36
5 | Segurança no Trabalho | 36
5 | Mecânica Aplicada | 72
5 | Administração e Organização Empresarial | 36
5 | Programação Estruturada | 36
5 | Programa em Plataforma Arduíno | 72
5 | Introdução à Teoria Econômica | 36
5 | Gerência de Projetos | 36
5 | Projetos e Atividades de Extensão III | 54
6 | Resistência dos Materiais I | 72
6 | Técnicas Digitais | 72
6 | Desenho Técnico II | 72
6 | Redes de Computadores | 72
6 | Fenômenos de Transporte | 36
6 | Eletrônica I | 72
6 | Projetos e Atividades de Extensão IV | 54
7 | Eletrônica II | 72
7 | Controle e Servomecanismos I | 72
7 | Laboratório de Tecnologia de Usinagem | 36
7 | Sistemas de Atuação | 72
7 | Tecnologia de Usinagem | 36
7 | Eletrônica Industrial | 72
7 | Projetos e Atividades de Extensão V | 54
8 | Resistência dos Materiais II | 72
8 | Microcontroladores | 72
8 | Ergonomia e Desenho Universal | 36
8 | Controle e Servomecanismos II | 72
8 | Laboratório de Controle e Servomecanismos | 36
8 | Projetos e Atividades de Extensão VI | 54
8 | Instrumentação I | 36
9 | Elementos de Máquinas I | 72
9 | Programação III | 72
9 | Automação de Processos Industriais | 72
9 | Metodologia Científica no Trabalho de Conclusão de Curso | 36
9 | Automação e Instrumentação Industrial | 36
9 | Projetos e Atividades de Extensão VII | 54
"""),
    "ARQ": ("Arquitetura e Urbanismo", """
1 | Expressão Gráfica | 72
1 | Plástica | 72
1 | Geografia Urbana | 36
1 | Introdução à Filosofia | 36
1 | Humanismo Cristão e Fenômeno Religioso | 72
1 | História e Arte | 36
1 | Introdução à Física | 36
1 | Desenho Técnico de Construção Civil | 72
2 | Ética | 36
2 | História da Arte – Arquitetura Média | 72
2 | Introdução ao Projeto de Arquitetura | 72
2 | Elementos de Arquitetura | 72
2 | Topografia | 72
2 | Tópicos de Teologia | 72
2 | Introdução a Sociologia | 36
3 | Projeto de Arquitetura I | 72
3 | História da Arte – Arquitetura Tardia | 72
3 | Introdução ao Cálculo | 72
3 | Desenho Computacional na Construção Civil | 72
3 | Gestão da Inovação | 36
3 | Sociologia Urbano-Ambiental | 36
3 | Geometria Descritiva | 36
3 | Projetos e Atividades de Extensão I | 54
4 | Geologia Geral | 36
4 | Projeto de Arquitetura II | 72
4 | História das Cidades | 36
4 | Instalações Elétricas | 72
4 | Instalações Hidráulicas | 72
4 | Direitos Humanos I | 36
4 | Projetos e Atividades de Extensão II | 54
5 | História e Cultura Afro-Brasileira | 72
5 | Mecânica dos Solos I | 72
5 | Projeto de Arquitetura III | 72
5 | Conforto Ambiental I | 72
5 | Teoria da Arquitetura e Urbanismo | 36
5 | Mecânica dos Materiais | 36
5 | Projetos e Atividades de Extensão III | 54
6 | Materiais de Construção Civil | 72
6 | Teoria das Estruturas I | 72
6 | Projeto de Arquitetura IV | 72
6 | Urbanismo I | 72
6 | Conforto Ambiental II | 72
6 | Projetos e Atividades de Extensão IV | 54
7 | Tecnologia da Construção Civil | 72
7 | Arquitetura de Interiores I | 72
7 | Paisagismo I | 72
7 | Urbanismo II | 72
7 | Projeto Integrado | 72
7 | Sistema Estrutural I | 72
7 | Projetos e Atividades de Extensão V | 54
8 | Arquitetura de Interiores II | 72
8 | Urbanismo III | 72
8 | Patrimônio I | 72
8 | Sistema Estrutural II | 72
8 | Laboratório de Materiais de Construção Civil | 36
8 | Paisagismo II | 72
8 | Projetos e Atividades de Extensão VI | 54
9 | Linguagem e Redação I | 72
9 | Fundações e Contenções de Encostas | 72
9 | Programação e Controle de Obras | 72
9 | Gerência de Projetos | 36
9 | Metodologia Científica no TCC | 36
9 | Patrimônio II | 36
9 | Tópicos Especiais de Arquitetura e Urbanismo | 36
9 | Projetos e Atividades de Extensão VII | 54
10 | Direito Ambiental | 36
10 | Ética Profissional em Arquitetura | 36
10 | Interculturalidade | 72
10 | Ergonomia e Desenho Universal | 36
10 | Trabalho de Conclusão de Curso | 18
10 | Projetos e Atividades de Extensão VIII | 54
10 | Estágio Supervisionado | 72
"""),
}


def creditos(horas: int) -> int:
    return round(horas / HORAS_POR_CREDITO)


def main():
    linhas = []  # lista de (codigo, nome, professor, periodo, creditos)

    # 1) Engenharia de Computação: copiado verbatim do CSV existente
    with open(ECOMP_EXTERNO, encoding="utf-8") as f:
        leitor = csv.reader(f)
        next(leitor)  # cabeçalho
        for r in leitor:
            if len(r) == 5 and r[0].strip():
                linhas.append((r[0], r[1], r[2], int(r[3]), int(r[4])))

    # 2) Demais cursos do CEC
    total_por_curso = {}
    for prefixo, (nome_curso, bloco) in CURSOS.items():
        n = 0
        for linha in bloco.strip().splitlines():
            partes = [p.strip() for p in linha.split("|")]
            if len(partes) != 3:
                continue
            periodo = int(re.match(r"\d+", partes[0]).group())
            nome = partes[1]
            horas = int(re.search(r"\d+", partes[2]).group())
            n += 1
            codigo = f"{prefixo}-{n:03d}"
            linhas.append((codigo, nome, "", periodo, creditos(horas)))
        total_por_curso[nome_curso] = n

    os.makedirs(os.path.dirname(SAIDA), exist_ok=True)
    with open(SAIDA, "w", encoding="utf-8", newline="") as f:
        w = csv.writer(f, quoting=csv.QUOTE_MINIMAL, lineterminator="\n")
        w.writerow(["codigo", "nome", "professor", "periodo", "creditos"])
        for cod, nome, prof, per, cred in linhas:
            w.writerow([cod, nome, prof, per, cred])

    print(f"Arquivo gerado: {SAIDA}")
    print(f"Total de disciplinas: {len(linhas)}")
    print("  Engenharia de Computação (existente): 72")
    for curso, n in total_por_curso.items():
        print(f"  {curso}: {n}")


if __name__ == "__main__":
    main()
