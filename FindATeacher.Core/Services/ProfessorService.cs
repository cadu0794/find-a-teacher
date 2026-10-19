using FindATeacher.Core.Models;
using System.Globalization;

namespace FindATeacher.Core.Services;

public class ProfessorService
{
    private readonly List<Professor> _professores;

    private static readonly CultureInfo Cultura =
        CultureInfo.InvariantCulture;

    public ProfessorService(List<Professor> professores)
    {
        _professores = professores;
    }

    public BuscaProfessoresResponse Buscar(BuscaProfessoresRequest request)
    {
        var resposta = new BuscaProfessoresResponse();

        int inicioFaixa = ConverterMinutos(request.InicioFaixa);
        int fimFaixa = ConverterMinutos(request.FimFaixa);

        foreach (var professor in _professores)
        {
            string? motivo = ObterMotivo(
                professor,
                request.Idioma,
                request.Dias,
                inicioFaixa,
                fimFaixa,
                request.DuracaoMinutos);

            if (motivo is not null)
            {
                resposta.NaoElegiveis.Add(new ProfessorNaoElegivelResponse
                {
                    Id = professor.Id,
                    Nome = professor.Nome,
                    Motivo = motivo
                });

                continue;
            }

            var horarios = ObterHorarios(
                professor,
                request.Dias,
                inicioFaixa,
                fimFaixa,
                request.DuracaoMinutos);

            resposta.Professores.Add(new ProfessorElegivelResponse
            {
                Id = professor.Id,
                Nome = professor.Nome,
                AulasNaSemana = professor.AulasMarcadas.Count,
                Horarios = horarios
            });
        }

        resposta.Professores = resposta.Professores
            .OrderBy(p => p.AulasNaSemana)
            .ThenBy(p => p.Nome, StringComparer.OrdinalIgnoreCase)
            .ToList();

        resposta.NaoElegiveis = resposta.NaoElegiveis
            .OrderBy(p => p.Id)
            .ToList();

        return resposta;
    }

    private static string? ObterMotivo(
        Professor professor,
        string idioma,
        List<string> dias,
        int inicioFaixa,
        int fimFaixa,
        int duracao)
    {
        if (!professor.Idiomas.Any(i =>
                string.Equals(i, idioma, StringComparison.OrdinalIgnoreCase)))
        {
            return "Não ensina o idioma";
        }

        if (professor.AgendaBloqueada)
        {
            return "Agenda bloqueada";
        }

        var horarios = ObterHorarios(
            professor, dias, inicioFaixa, fimFaixa, duracao);

        if (horarios.Count == 0)
        {
            return "Sem horário em comum nos dias pedidos";
        }

        return null;
    }

    private static List<string> ObterHorarios(
        Professor professor,
        List<string> dias,
        int inicioFaixa,
        int fimFaixa,
        int duracao)
    {
        var horarios = new List<string>();

        int primeiroInicio = ((inicioFaixa + 29) / 30) * 30;

        for (int inicio = primeiroInicio;
            inicio + duracao <= fimFaixa;
            inicio += 30)
        {
            bool disponivelTodosOsDias = dias.All(dia =>
                PodeDarAula(professor, dia, inicio, duracao));

            if (disponivelTodosOsDias)
            {
                horarios.Add(
                    TimeSpan.FromMinutes(inicio).ToString(@"hh\:mm"));
            }
        }
        return horarios;
    }

    private static bool PodeDarAula(
        Professor professor,
        string dia,
        int inicio,
        int duracao)
    {
        int fim = inicio + duracao;

        bool dentroDaDisponibilidade = professor.Disponibilidade.Any(d =>
        {
            if (DiaSemanaHelper.Normalizar(d.Dia)
                != DiaSemanaHelper.Normalizar(dia))
            {
                return false;
            }

            int inicioDisponibilidade = ConverterMinutos(d.Inicio);
            int fimDisponibilidade = ConverterMinutos(d.Fim);

            return inicio >= inicioDisponibilidade
                && fim <= fimDisponibilidade;
        });

        if (!dentroDaDisponibilidade)
        {
            return false;
        }

        bool conflitaComAula = professor.AulasMarcadas.Any(a =>
        {
            if (DiaSemanaHelper.Normalizar(a.Dia)
                != DiaSemanaHelper.Normalizar(dia))
            {
                return false;
            }

            int inicioAula = ConverterMinutos(a.Inicio);
            int fimAula = inicioAula + a.DuracaoMinutos;

            // Intervalos se sobrepõem somente se houver interseção real.
            return inicio < fimAula && fim > inicioAula;
        });

        return !conflitaComAula;
    }

    private static int ConverterMinutos(string horario)
    {
        var time = TimeSpan.ParseExact(horario, @"hh\:mm", Cultura);
        return (int)time.TotalMinutes;
    }
}