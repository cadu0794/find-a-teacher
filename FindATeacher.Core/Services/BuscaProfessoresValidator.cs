
using FindATeacher.Core.Models;
using System.Globalization;

namespace FindATeacher.Core.Services;

public class BuscaProfessoresValidator
{
    private static readonly string[] DiasValidos =
    [
        "segunda", "terca", "quarta", "quinta",
        "sexta", "sabado", "domingo"
    ];

    public List<string> Validar(BuscaProfessoresRequest request)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Idioma))
        {
            erros.Add("O idioma é obrigatório.");
        }

        if (request.Dias is null || request.Dias.Count == 0)
        {
            erros.Add("Informe pelo menos um dia da semana.");
        }
        else if (request.Dias.Any(d =>
            !DiasValidos.Contains(
                DiaSemanaHelper.Normalizar(d))))
        {
            erros.Add("Um ou mais dias da semana são inválidos.");
        }

        bool inicioValido = TentarConverterMinutos(
            request.InicioFaixa, out int inicio);

        bool fimValido = TentarConverterMinutos(
            request.FimFaixa, out int fim);

        if (!inicioValido)
        {
            erros.Add("O horário inicial deve estar no formato HH:mm.");
        }

        if (!fimValido)
        {
            erros.Add("O horário final deve estar no formato HH:mm.");
        }

        if (inicioValido && fimValido && fim <= inicio)
        {
            erros.Add("O horário final deve ser posterior ao inicial.");
        }

        if (request.DuracaoMinutos is not (30 or 60 or 90))
        {
            erros.Add("A duração deve ser de 30, 60 ou 90 minutos.");
        }

        return erros;
    }

    private static bool TentarConverterMinutos(
        string? horario, out int minutos)
    {
        minutos = 0;

        if (string.IsNullOrWhiteSpace(horario))
        {
            return false;
        }

        if (!TimeSpan.TryParseExact(
                horario,
                @"hh\:mm",
                CultureInfo.InvariantCulture,
                out var time))
        {
            return false;
        }

        if (time < TimeSpan.Zero || time >= TimeSpan.FromDays(1))
        {
            return false;
        }

        minutos = (int)time.TotalMinutes;
        return true;
    }
}
