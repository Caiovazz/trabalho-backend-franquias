Gestão de Franquias

API acadêmica para gerenciamento de uma rede de franquias.

O projeto permite controlar franqueadoras, unidades, usuários, produtos, fornecedores, estoque, vendas, royalties e chamados de suporte.

Tecnologias utilizadas

- C#
- ASP.NET Core
- .NET 10
- Entity Framework Core
- MySQL 8
- Autenticação JWT
- Swagger/OpenAPI
- Git e GitHub

Funcionalidades

- Cadastro e autenticação de usuários.
- Perfis de Administrador, Gestor e Operador.
- Cadastro e inativação de franqueadoras.
- CRUD de unidades franqueadas.
- Cadastro de responsáveis pelas unidades.
- CRUD de categorias, produtos e fornecedores.
- Controle de entrada e saída de estoque.
- Bloqueio de estoque negativo.
- Cadastro de vendas e cálculo automático do total.
- Atualização automática do estoque após uma venda.
- Configuração e cálculo de royalties.
- Abertura e encerramento de chamados.
- Relatórios de faturamento, royalties, estoque e vendas.
- Filtros e paginação nos principais endpoints.

Estrutura principal

Franquias.Api/
├── Controllers/
├── DTOs/
├── Entities/
├── Services/
├── FranquiasDbContext.cs
├── Startup.cs
└── Program.cs

database/
├── franquiasDB-schema.sql
└── dados-exemplo.sql

docs/
└── franqueadoras.http

Requisitos

Para executar o projeto é necessário possuir:

- .NET SDK 10
- MySQL Server 8
- Git
- Um SGBD para gerenciamento do banco de dados

SGBD recomendado

Recomenda-se utilizar o MySQL Workbench, pois o projeto utiliza MySQL e o Workbench oferece melhor compatibilidade para executar e visualizar o banco de dados.

---

Configuração do banco de dados

Para executar o projeto, primeiro é necessário criar um banco de dados vazio no seu SGBD.

Não é necessário criar manualmente as tabelas, relacionamentos ou outras estruturas do banco.

1. Criar um banco vazio

No MySQL Workbench, crie apenas o banco de dados que será utilizado pela API.

Por exemplo:

CREATE DATABASE teste1;

«O nome do banco pode ser diferente. Nesse caso, utilize o mesmo nome na configuração da aplicação.»

2. Configurar a conexão da API

Depois de criar o banco vazio, abra o arquivo:

Franquias.Api/appsettings.json

Localize a configuração "ConnectionStrings" e informe os dados do seu banco.

Exemplo:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=teste1;User=root;Password=;"
}

Altere os valores conforme a configuração do seu ambiente:

- "Server": endereço do servidor MySQL.
- "Port": porta utilizada pelo MySQL, normalmente "3306".
- "Database": nome do banco vazio criado anteriormente.
- "User": usuário do MySQL.
- "Password": senha do usuário do MySQL.

Por exemplo, caso o banco se chame "franquiasDB" e o usuário seja "root":

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=franquiasDB;User=root;Password=SUA_SENHA;"
}

3. Executar a API

Depois de configurar a conexão, execute o projeto:

dotnet build .\Franquias.Api\Franquias.Api.csproj

Depois:

dotnet run --project .\Franquias.Api\Franquias.Api.csproj --launch-profile "Franquias.Api"

Ao iniciar a API, o Entity Framework Core criará automaticamente as tabelas e os relacionamentos necessários no banco de dados configurado.

Portanto, o banco criado inicialmente precisa estar vazio. A API será responsável por criar sua estrutura.

4. Popular o banco com dados de exemplo

Depois que a API for executada e as tabelas forem criadas, utilize o arquivo:

database/dados-exemplo.sql

Abra o arquivo no MySQL Workbench, conecte-se ao mesmo banco configurado no "appsettings.json" e execute os comandos SQL.

Esses comandos irão inserir os dados de demonstração necessários para testar a aplicação.

O fluxo completo é:

Criar banco vazio
       ↓
Configurar appsettings.json
       ↓
Subir a API
       ↓
Entity Framework cria as tabelas
       ↓
Abrir dados-exemplo.sql
       ↓
Executar os comandos no MySQL Workbench
       ↓
Banco populado
       ↓
API pronta para utilização

Depois disso, a API poderá ser utilizada normalmente pelo Swagger e pelos demais clientes HTTP.

---

Como executar

Na pasta principal do repositório, execute:

dotnet build .\Franquias.Api\Franquias.Api.csproj

Depois:

dotnet run --project .\Franquias.Api\Franquias.Api.csproj --launch-profile "Franquias.Api"

A documentação da API estará disponível em:

https://localhost:7051/swagger/index.html

Para encerrar a API, pressione "Ctrl + C".

Autenticação

O login é realizado por:

POST /api/Auth/login

O token recebido deve ser informado no botão "Authorize" do Swagger.

Os dados de exemplo incluem um usuário de demonstração:

E-mail: admin.exemplo@franquias.local
Senha: Admin123!

Esse usuário deve ser utilizado somente em ambiente local de desenvolvimento.

Principais endpoints

Módulo| Endpoints
Autenticação| "/api/Auth"
Franqueadoras| "/api/franqueadoras"
Unidades| "/api/Unidades"
Responsáveis| "/api/Responsaveis"
Categorias| "/api/Categorias"
Produtos| "/api/Produtos"
Fornecedores| "/api/Fornecedores"
Estoque| "/api/Estoque"
Vendas| "/api/Vendas"
Royalties| "/api/Royalty"
Chamados| "/api/Chamados"
Relatórios| "/api/Relatorios"

Regras de negócio implementadas

- CNPJ de franqueadoras, unidades e fornecedores não pode ser duplicado.
- Unidades, usuários, produtos e franqueadoras podem ser inativados.
- Unidades inativas não podem realizar vendas.
- Produtos inativos não podem ser vendidos.
- O estoque não pode ficar negativo.
- A venda calcula automaticamente o valor total.
- A venda desconta automaticamente os produtos do estoque.
- O royalty é calculado sobre o faturamento da unidade.
- Chamados possuem prioridade, situação e data de encerramento.
- Endpoints protegidos exigem autenticação e perfil autorizado.

Banco de dados

O projeto utiliza Entity Framework Core com MySQL.

As tabelas e seus relacionamentos são criados automaticamente pela aplicação a partir da configuração do Entity Framework Core.

As principais relações incluem:

- Franqueadora e unidades.
- Unidade e responsáveis.
- Categoria, fornecedor e produtos.
- Unidade, produto e estoque.
- Venda e itens da venda.
- Unidade e cobranças de royalty.
- Unidade, usuário e chamados.

Testes

Os endpoints foram testados manualmente pelo Swagger, incluindo:

- Login e autorização JWT.
- Cadastro e inativação de usuários.
- Cadastro e inativação de franqueadoras e unidades.
- Cadastro de responsáveis.
- Cadastro de produtos e fornecedores.
- Entrada e saída de estoque.
- Bloqueio de saída com saldo insuficiente.
- Cadastro de venda e atualização do estoque.
- Cálculo e pagamento de royalties.
- Abertura e encerramento de chamados.
- Consultas e relatórios.

Repositório

O desenvolvimento das funcionalidades é realizado na branch:

desenvolvimento

Após os testes, a versão final será integrada à branch:

main
