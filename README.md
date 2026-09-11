# 🏢 Sistema de Governança de Franquias

API RESTful desenvolvida para a **administração centralizada e o controle operacional de redes de franquias**.

Este projeto universitário foi desenvolvido em **C#**, utilizando a plataforma **ASP.NET Core**, e tem como objetivo estruturar uma solução para gerenciamento de franqueadoras, unidades, usuários e demais operações relacionadas à rede de franquias.

---

## 🛠️ Stack Tecnológica

| Tecnologia | Utilização |
|---|---|
| **C#** | Linguagem de programação |
| **.NET 10** | Plataforma de desenvolvimento |
| **ASP.NET Core Web API** | Desenvolvimento da API RESTful |
| **Swagger / OpenAPI** | Documentação e testes dos endpoints |
| **Git & GitHub** | Controle de versão e hospedagem do código |
| **MySQL** | Banco de dados |
| **Entity Framework Core** | ORM e persistência de dados |

> **Status da persistência:** a integração com MySQL e Entity Framework Core está prevista para as próximas etapas do projeto.

---

## 🏗️ Arquitetura e Estágio Atual

Atualmente, a API possui um fluxo inicial destinado à **validação e sanitização dos dados cadastrais da franqueadora principal**.

Nesta primeira etapa, os dados recebidos são processados **em memória**, sem persistência no banco de dados.

### ✅ Validações Ativas

- Verificação da presença do campo **Nome/Razão Social**;
- Verificação da presença do **CNPJ**;
- Verificação da obrigatoriedade do **e-mail**;
- Validação do formato sintático do e-mail.

### 🔜 Próximas Validações

Nas próximas etapas serão implementadas:

- Validação matemática dos dígitos verificadores do CNPJ;
- Verificação de duplicidade no banco de dados;
- Persistência definitiva dos dados.

---

## 📋 Pré-requisitos

Para executar a versão atual do projeto, é necessário possuir:

- **.NET 10 SDK** instalado.

> 💡 **Observação:** não é necessário possuir uma instância do **MySQL** em execução para utilizar esta versão preliminar da aplicação.

---

## 🚀 Como Executar

### 1. Acesse a raiz do projeto

Abra um terminal na pasta:

```text
trabalho-backend-franquias
```

### 2. Execute a aplicação

```bash
dotnet run --project ./Franquias.Api/Franquias.Api.csproj --launch-profile "Franquias.Api"
```

### 3. Acesse o Swagger

Com a aplicação em execução, abra:

[Swagger UI — Franquias API](https://localhost:7051/swagger/index.html?utm_source=chatgpt.com)

Através do Swagger é possível visualizar a documentação da API e realizar testes diretamente pelo navegador.

### 4. Encerrar a aplicação

Para interromper o servidor, utilize:

```text
Ctrl + C
```

---

## 🔗 Mapeamento de Rotas

### 🏢 Checagem Cadastral

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/franqueadoras/validar` | Valida os dados cadastrais da franqueadora |

### Exemplo de requisição

```http
POST /api/franqueadoras/validar
Content-Type: application/json
```

```json
{
  "nome": "Franqueadora Exemplo LTDA",
  "cnpj": "00.000.000/0001-00",
  "email": "contato@exemplo.com"
}
```

---

## 🧪 Registro de Testes Operacionais

Os testes iniciais foram realizados através do **Swagger UI**.

| Cenário | Resultado |
|---|---|
| Razão Social, CNPJ e e-mail válidos | `200 OK` |
| Campos obrigatórios ausentes | `400 Bad Request` |
| E-mail em formato inválido | `400 Bad Request` |

> ⚠️ **Importante:** o código HTTP `200 OK` indica apenas que a requisição atendeu às regras de negócio implementadas nesta etapa. Isso **não significa que os dados foram gravados no banco de dados**.

---

## 🗺️ Roadmap

O desenvolvimento do sistema está planejado em etapas:

### 💾 Persistência
- [ ] Configuração do **Entity Framework Core**;
- [ ] Configuração da conexão com **MySQL**;
- [ ] Criação e gerenciamento das **migrations**;
- [ ] Persistência dos dados cadastrais.

### 🏢 Gestão de Franquias
- [ ] CRUD de **Franqueadoras**;
- [ ] CRUD de **Filiais/Unidades**;
- [ ] CRUD de **Gestores**.

### 🔐 Identidade e Segurança
- [ ] Implementação de **Identidade**;
- [ ] Autenticação;
- [ ] Autorização;
- [ ] Controle de acesso baseado em funções (**RBAC**).

### 📦 Produtos e Fornecedores
- [ ] Gestão do catálogo de produtos;
- [ ] Gestão de categorias;
- [ ] Gestão da rede de fornecedores.

### 💰 Operações
- [ ] Módulo de vendas;
- [ ] Controle de estoque das unidades;
- [ ] Mapeamento e cobrança de royalties.

### 🎧 Atendimento
- [ ] Central de atendimento;
- [ ] Sistema de chamados (**Helpdesk**).

### 📊 Indicadores e Relatórios
- [ ] Dashboard de indicadores de desempenho;
- [ ] Relatórios analíticos;
- [ ] Visualização de métricas da rede de franquias.

---

## 📌 Status do Projeto

**Em desenvolvimento 🚧**

O projeto encontra-se em sua fase inicial, com a estrutura da API e as primeiras regras de validação implementadas. As próximas etapas estão concentradas na integração com o banco de dados, persistência das informações e expansão dos módulos de negócio.
