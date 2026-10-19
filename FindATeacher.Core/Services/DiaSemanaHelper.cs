
using System.Globalization;
using System.Text;

namespace FindATeacher.Core.Services;

public static class DiaSemanaHelper
{
    public static string Normalizar(string dia)
    {
        string normalizado = dia.Trim().Normalize(
            NormalizationForm.FormD);

        var resultado = new StringBuilder();

        foreach (char caractere in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere)
                != UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(caractere);
            }
        }

        return resultado.ToString().ToLowerInvariant();
    }
}
