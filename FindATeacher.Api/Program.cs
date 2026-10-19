
using System.Text.Json;
using FindATeacher.Core.Models;
using FindATeacher.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var caminhoArquivo = Path.Combine(
    AppContext.BaseDirectory,
    "professores.json");

if (!File.Exists(caminhoArquivo))
{
    throw new FileNotFoundException(
        "O arquivo professores.json não foi encontrado.",
        caminhoArquivo);
}

var json = File.ReadAllText(caminhoArquivo);

var dados = JsonSerializer.Deserialize<ProfessoresArquivo>(
    json,
    new JsonSerializerOptions(JsonSerializerDefaults.Web));

if (dados?.Professores is null)
{
    throw new InvalidOperationException(
        "Não foi possível carregar a lista de professores.");
}

builder.Services.AddSingleton(
    new ProfessorService(dados.Professores));

builder.Services.AddSingleton<BuscaProfessoresValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost(
    "/api/professores/busca",
    (
        BuscaProfessoresRequest request,
        ProfessorService professorService,
        BuscaProfessoresValidator validator
    ) =>
    {
        var erros = validator.Validar(request);

        if (erros.Count > 0)
        {
            return Results.BadRequest(new
            {
                erros
            });
        }

        var resultado = professorService.Buscar(request);

        return Results.Ok(resultado);
    })
    .WithName("BuscarProfessores")
    .WithSummary("Busca professores disponíveis")
    .WithOpenApi();

app.Run();
