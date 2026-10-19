namespace FindATeacher.Core.Models;

public class BuscaProfessoresResponse
{
    public List<ProfessorElegivelResponse> Professores { get; set; } = [];
    public List<ProfessorNaoElegivelResponse> NaoElegiveis { get; set; } = [];
}

public class ProfessorElegivelResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AulasNaSemana { get; set; }
    public List<string> Horarios { get; set; } = [];
}

public class ProfessorNaoElegivelResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}