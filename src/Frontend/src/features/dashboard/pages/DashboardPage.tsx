import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Award, BookOpen, GraduationCap, ScrollText } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ErrorState } from "@/components/feedback/ErrorState";
import { Loading } from "@/components/feedback/Loading";
import { obterMensagemDeErro } from "@/core/errors/api-error";
import { useSessao } from "@/core/session/useSessao";
import { historicoApi } from "@/features/historico/services/historico-api";
import type { CrDto } from "@/types/api";
import { formatarCr } from "@/lib/formatters";
import { paths } from "@/routes/paths";

export function DashboardPage() {
  const { aluno } = useSessao();
  const [cr, setCr] = useState<CrDto | null>(null);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      setCr(await historicoApi.obterCr());
    } catch (e) {
      setErro(obterMensagemDeErro(e));
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    void carregar();
  }, []);

  if (carregando) return <Loading mensagem="Carregando painel..." />;
  if (erro) return <ErrorState mensagem={erro} onTentarNovamente={carregar} />;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Olá, {aluno?.nome?.split(" ")[0]}!</h1>
        <p className="text-sm text-muted-foreground">Acompanhe seu desempenho no curso.</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <Card className="border-primary/30 bg-primary/5">
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-2">
              <Award className="h-4 w-4 text-primary" />
              Coeficiente de Rendimento
            </CardDescription>
            <CardTitle className="text-4xl text-primary">{formatarCr(cr?.cr)}</CardTitle>
          </CardHeader>
          <CardContent className="text-xs text-muted-foreground">
            Média ponderada pelos créditos, com 2 casas decimais.
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-2">
              <ScrollText className="h-4 w-4" />
              Disciplinas cursadas
            </CardDescription>
            <CardTitle className="text-4xl">{cr?.totalDisciplinas ?? 0}</CardTitle>
          </CardHeader>
          <CardContent className="text-xs text-muted-foreground">
            Lançamentos no seu histórico acadêmico.
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-2">
              <GraduationCap className="h-4 w-4" />
              Créditos acumulados
            </CardDescription>
            <CardTitle className="text-4xl">{cr?.totalCreditos ?? 0}</CardTitle>
          </CardHeader>
          <CardContent className="text-xs text-muted-foreground">
            Soma dos créditos das disciplinas com nota.
          </CardContent>
        </Card>
      </div>

      <div className="flex flex-wrap gap-3">
        <Link
          to={paths.disciplinas}
          className="inline-flex h-9 items-center gap-2 rounded-md border border-border bg-card px-4 text-sm font-medium hover:bg-accent"
        >
          <BookOpen className="h-4 w-4" /> Ver disciplinas
        </Link>
        <Link
          to={paths.historico}
          className="inline-flex h-9 items-center gap-2 rounded-md border border-border bg-card px-4 text-sm font-medium hover:bg-accent"
        >
          <ScrollText className="h-4 w-4" /> Ver histórico
        </Link>
      </div>
    </div>
  );
}
