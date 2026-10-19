
# Find a Teacher

API REST em C#/.NET 8 e interface em React + TypeScript para encontrar professores de idiomas disponíveis de acordo com os dias da semana, a faixa de horário e a duração da aula.

## Tecnologias

### Backend
- C# e .NET 8
- ASP.NET Core Minimal APIs
- xUnit para testes automatizados
- JSON para carregar os dados dos professores

### Frontend
- React
- TypeScript
- Vite
- ESLint
- CSS responsivo

## Funcionalidades

- Busca de professores por idioma.
- Seleção de um ou mais dias da semana.
- Definição de uma faixa de horário.
- Escolha da duração da aula: 30, 60 ou 90 minutos.
- Exibição dos professores elegíveis e seus horários disponíveis.
- Exibição dos motivos de inelegibilidade.
- Validação dos dados da busca.
- Indicadores de carregamento, erros e ausência de resultados.

## Estrutura do projeto

```text
find-a-teacher/
├── FindATeacher.Api/
│   └── Program.cs
├── FindATeacher.Core/
│   ├── Models/
│   └── Services/
├── FindATeacher.Tests/
├── frontend/
│   └── src/
├── professores.json
├── FindATeacher.sln
├── README.md
└── USO_DE_IA.md
```

## Pré-requisitos

- .NET 8 SDK
- Node.js e npm
- Git (opcional, para versionamento)

## Como executar o backend

Na raiz do projeto, execute:

```powershell
dotnet restore
dotnet run --project .\FindATeacher.Api
```

A API estará disponível no endereço informado pelo terminal. Na configuração local utilizada durante o desenvolvimento, o endereço é:

`http://localhost:5200`

A documentação Swagger pode ser acessada em:

`http://localhost:5200/swagger`

O arquivo `professores.json` fica na raiz do projeto e é copiado para a saída de compilação da API.

## Como executar o frontend

Abra um segundo terminal e execute:

```powershell
cd .\frontend
npm install
npm run dev
```

Acesse o endereço exibido pelo Vite, normalmente:

`http://localhost:5173`

O frontend utiliza o proxy configurado em `frontend/vite.config.ts` para encaminhar as requisições `/api` para o backend em `http://localhost:5200`.

Mantenha os dois servidores em execução durante o uso local.

## Endpoint

### POST /api/professores/busca

Exemplo de requisição:

```json
{
  "idioma": "Inglês",
  "dias": ["segunda", "quarta"],
  "inicioFaixa": "14:00",
  "fimFaixa": "18:00",
  "duracaoMinutos": 60
}
```

A resposta contém duas listas:

- `professores`: professores elegíveis, quantidade de aulas na semana e horários disponíveis.
- `naoElegiveis`: professores não elegíveis e o motivo correspondente.

Requisições inválidas retornam HTTP 400 com mensagens de validação.

## Regras de negócio e decisões

- O professor precisa ensinar o idioma solicitado.
- Professores com agenda bloqueada são considerados não elegíveis.
- O mesmo horário de início deve estar disponível em todos os dias selecionados.
- A aula deve caber integralmente na faixa de horário informada e em um período de disponibilidade do professor.
- Os horários candidatos são gerados em intervalos de 30 minutos, começando em `:00` ou `:30`.
- Aulas já marcadas não podem ter sobreposição com a nova aula. Uma aula pode começar exatamente quando outra termina.
- São aceitas aulas de 30, 60 ou 90 minutos.
- A validação exige idioma, pelo menos um dia, horários válidos e horário final posterior ao inicial.
- Os motivos de inelegibilidade seguem esta prioridade:
  1. `Não ensina o idioma`
  2. `Agenda bloqueada`
  3. `Sem horário em comum nos dias pedidos`
- Os professores elegíveis são ordenados pela menor quantidade de aulas agendadas na semana e, em caso de empate, pelo nome.
- Os horários disponíveis são apresentados em ordem crescente.
- Os dados são carregados do JSON e mantidos em memória durante a execução da API.

## Testes automatizados

Na raiz do projeto, execute:

```powershell
dotnet test
```

Os testes existentes cobrem elegibilidade por idioma, agenda bloqueada, conflito com aulas marcadas, disponibilidade em todos os dias solicitados e ordenação dos resultados.

## Limitações e melhorias futuras

- Os dados dos professores são estáticos e carregados de um arquivo JSON.
- Não há persistência em banco de dados nem cadastro de professores.
- Não há autenticação de usuários.
- A busca é somente para consulta de disponibilidade; não realiza agendamento de aulas.
- A cobertura de testes pode ser ampliada para validações de entrada e mais casos de fronteira.

## Uso de inteligência artificial

Consulte o arquivo [USO_DE_IA.md](USO_DE_IA.md) para conhecer as ferramentas de IA utilizadas durante o desenvolvimento, como foram empregadas e um exemplo de problema identificado e corrigido.
