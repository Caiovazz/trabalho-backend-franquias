# Gestão de Franquias

Projeto acadêmico de uma API para gestão de franquias,
desenvolvido em C# com ASP.NET Core.

## Tecnologias

- C#
- ASP.NET Core com .NET 10
- Swagger/OpenAPI
- Git e GitHub
- PostgreSQL e Entity Framework Core: integração planejada

## Estado atual

A API possui um endpoint inicial para validar os dados
de uma franqueadora.

São verificados:
- Preenchimento do nome.
- Preenchimento do CNPJ.
- Preenchimento e formato do e-mail.

Nesta etapa, o endpoint devolve os dados recebidos,
sem gravá-los no banco.

A validação dos dígitos do CNPJ e a verificação de
registros duplicados ainda serão implementadas.

## Requisitos

- SDK do .NET 10 instalado.

A versão inicial pode ser executada sem configurar o PostgreSQL.

## Como executar

Abra o terminal na pasta principal gestao-franquias e execute:

```powershell
dotnet run --project .\Franquias.Api\Franquias.Api.csproj --launch-profile "Franquias.Api"
```

Mantenha o terminal aberto enquanto utiliza a API.

Acesse o [Swagger](https://localhost:7051/swagger/index.html).

Para encerrar a API, pressione Ctrl + C no terminal.

## Endpoint de validação

POST /api/franqueadoras/validar

## Testes manuais realizados no Swagger

| Cenário | Resultado observado |
|---|---|
| Nome e CNPJ preenchidos e e-mail com formato válido | 200 OK |
| Nome e CNPJ vazios e e-mail com formato inválido | 400 Bad Request |

O resultado 200 confirma que os dados passaram pelas
validações atuais. Não representa um cadastro no banco.

## Próximas implementações

- Integração com PostgreSQL usando Entity Framework Core.
- Cadastros de franqueadoras, unidades e responsáveis.
- Usuários, autenticação e autorização.
- Produtos, categorias e fornecedores.
- Estoque e vendas.
- Royalties.
- Chamados de suporte.
- Indicadores e consultas.