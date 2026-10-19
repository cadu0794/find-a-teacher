
# Uso de Inteligência Artificial

## Ferramentas utilizadas

- ChatGPT: apoio à organização das etapas, implementação das regras de negócio, criação e revisão de código, testes automatizados, configuração do frontend e documentação.

## Como a IA foi utilizada

A IA foi utilizada como ferramenta de apoio durante o desenvolvimento do Find a Teacher, principalmente para:

- Planejar a separação entre API, regras de negócio e testes.
- Auxiliar na criação dos modelos e serviços em C#.
- Discutir as regras de disponibilidade e os motivos de inelegibilidade.
- Criar e revisar testes automatizados com xUnit.
- Orientar a configuração do React com TypeScript e Vite.
- Auxiliar na integração entre frontend e backend.
- Preparar a documentação do projeto.

As sugestões foram aplicadas de forma incremental, com execução dos comandos e verificação do comportamento do projeto durante o desenvolvimento.

## Exemplo de erro ou limitação identificada

Durante a criação do frontend com Vite, a instalação das dependências falhou com o erro `ERESOLVE`.

O relatório do npm mostrou uma incompatibilidade entre as versões: o projeto estava tentando utilizar TypeScript 7.0.2, enquanto a dependência `typescript-eslint` instalada exigia uma versão de TypeScript inferior a 6.1.0.

A orientação inicial para criar o frontend não antecipou esse conflito específico de dependências.

### Como o problema foi identificado e corrigido

1. Observei que o comando de instalação terminou com erro, apesar de a estrutura do frontend ter sido criada.
2. Li a mensagem `ERESOLVE` e identifiquei a incompatibilidade indicada pelo npm.
3. Ajustei a versão do TypeScript no `package.json` para `~5.9.3`, compatível com o intervalo exigido pela dependência.
4. Executei novamente `npm install` e confirmei que o frontend iniciou corretamente.

Esse caso demonstrou a importância de verificar as mensagens do compilador e do gerenciador de pacotes, em vez de assumir que uma sugestão da IA ou uma configuração gerada automaticamente está sempre correta.

## Validação do trabalho

Durante o desenvolvimento, utilizei os testes automatizados do .NET, a execução local da API, o Swagger e a interface React para verificar o comportamento da aplicação.