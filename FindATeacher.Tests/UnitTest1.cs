
using FindATeacher.Core.Models;
using FindATeacher.Core.Services;

namespace FindATeacher.Tests;

public class ProfessorServiceTests
{
    private static Professor CriarProfessor(
        int id = 1,
        string nome = "Ana",
        List<string>? idiomas = null,
        bool agendaBloqueada = false,
        List<Disponibilidade>? disponibilidade = null,
        List<AulaMarcada>? aulasMarcadas = null)
    {
        return new Professor
        {
            Id = id,
            Nome = nome,
            Idiomas = idiomas ?? ["Inglês"],
            AgendaBloqueada = agendaBloqueada,
            Disponibilidade = disponibilidade ??
            [
                new Disponibilidade
                {
                    Dia = "segunda",
                    Inicio = "14:00",
                    Fim = "18:00"
                }
            ],
            AulasMarcadas = aulasMarcadas ?? []
        };
    }

    private static BuscaProfessoresRequest CriarRequest(
        string idioma = "Inglês",
        List<string>? dias = null,
        string inicio = "14:00",
        string fim = "18:00",
        int duracao = 60)
    {
        return new BuscaProfessoresRequest
        {
            Idioma = idioma,
            Dias = dias ?? ["segunda"],
            InicioFaixa = inicio,
            FimFaixa = fim,
            DuracaoMinutos = duracao
        };
    }

    [Fact]
    public void Buscar_DeveRetornarProfessorQueEnsinaIdioma()
    {
        var professor = CriarProfessor();
        var service = new ProfessorService([professor]);

        var resultado = service.Buscar(CriarRequest());

        Assert.Single(resultado.Professores);
        Assert.Empty(resultado.NaoElegiveis);
    }

    [Fact]
    public void Buscar_DeveExcluirProfessorQueNaoEnsinaIdioma()
    {
        var professor = CriarProfessor(idiomas: ["Espanhol"]);
        var service = new ProfessorService([professor]);

        var resultado = service.Buscar(CriarRequest());

        Assert.Empty(resultado.Professores);
        Assert.Equal(
            "Não ensina o idioma",
            Assert.Single(resultado.NaoElegiveis).Motivo);
    }

    [Fact]
    public void Buscar_DeveExcluirAgendaBloqueada()
    {
        var professor = CriarProfessor(agendaBloqueada: true);
        var service = new ProfessorService([professor]);

        var resultado = service.Buscar(CriarRequest());

        Assert.Empty(resultado.Professores);
        Assert.Equal(
            "Agenda bloqueada",
            Assert.Single(resultado.NaoElegiveis).Motivo);
    }

    [Fact]
    public void Buscar_DeveIgnorarHorarioQueConflitaComAulaMarcada()
    {
        var professor = CriarProfessor(
            aulasMarcadas:
            [
                new AulaMarcada
                {
                    Dia = "segunda",
                    Inicio = "15:00",
                    DuracaoMinutos = 60
                }
            ]);

        var service = new ProfessorService([professor]);

        var resultado = service.Buscar(
            CriarRequest(inicio: "14:00", fim: "17:00"));

        Assert.Equal(["14:00", "16:00"], 
            Assert.Single(resultado.Professores).Horarios);
    }

    [Fact]
    public void Buscar_DeveExigirDisponibilidadeEmTodosOsDias()
    {
        var professor = CriarProfessor(
            disponibilidade:
            [
                new Disponibilidade
                {
                    Dia = "segunda",
                    Inicio = "14:00",
                    Fim = "18:00"
                },
                new Disponibilidade
                {
                    Dia = "quarta",
                    Inicio = "16:00",
                    Fim = "18:00"
                }
            ]);

        var service = new ProfessorService([professor]);

        var resultado = service.Buscar(
            CriarRequest(dias: ["segunda", "quarta"]));

        Assert.Equal(["16:00", "16:30", "17:00"],
            Assert.Single(resultado.Professores).Horarios);
    }

    [Fact]
    public void Buscar_DeveOrdenarPorQuantidadeDeAulasENome()
    {
        var professores = new List<Professor>
        {
            CriarProfessor(
                id: 1,
                nome: "Bruno",
                aulasMarcadas:
                [
                    new AulaMarcada
                    {
                        Dia = "terça",
                        Inicio = "10:00",
                        DuracaoMinutos = 60
                    }
                ]),
            CriarProfessor(id: 2, nome: "Carlos"),
            CriarProfessor(id: 3, nome: "Ana")
        };

        var service = new ProfessorService(professores);

        var resultado = service.Buscar(CriarRequest());

        Assert.Equal(
            ["Ana", "Carlos", "Bruno"],
            resultado.Professores.Select(p => p.Nome).ToArray());
    }
}
