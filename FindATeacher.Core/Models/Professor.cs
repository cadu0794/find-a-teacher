
namespace FindATeacher.Core.Models;

public class Professor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<string> Idiomas { get; set; } = [];
    public bool AgendaBloqueada { get; set; }
    public List<Disponibilidade> Disponibilidade { get; set; } = [];
    public List<AulaMarcada> AulasMarcadas { get; set; } = [];
}

public class Disponibilidade
{
    public string Dia { get; set; } = string.Empty;
    public string Inicio { get; set; } = string.Empty;
    public string Fim { get; set; } = string.Empty;
}

public class AulaMarcada
{
    public string Dia { get; set; } = string.Empty;
    public string Inicio { get; set; } = string.Empty;
    public int DuracaoMinutos { get; set; }
}
