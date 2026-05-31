# API de Gerenciamento de Pessoas 👥

**Status:** 🟢 Concluído  
**Última Atualização:** 31 de Maio de 2026  
**Data de Entrega:** 2 de Junho de 2026

---

## 📋 Visão Geral do Projeto

Uma API Web RESTful construída com **ASP.NET Core 8** para gerenciar pessoas e seus endereços. A API demonstra padrões profissionais de arquitetura de software, boas práticas de código e integração abrangente com banco de dados.

**Propósito:** Avaliação técnica para validar organização do código, boas práticas, design de API e compreensão do desenvolvedor sobre a implementação.

---

## 🛠️ Stack Tecnológico

| Tecnologia | Versão | Propósito |
|------------|--------|----------|
| **.NET** | 8.0 | Framework |
| **ASP.NET Core** | 8.0 | Web API |
| **Entity Framework Core** | 8.0 | ORM (Mapeamento Relacional de Objetos) |
| **SQL Server** | 2019+ | Banco de Dados |
| **Swagger/OpenAPI** | 6.6.2 | Documentação & Testes da API |
| **C#** | 12 | Linguagem |

---

## 🏗️ Estrutura do Projeto

```
person-management-api/
├── src/
│   └── Api/
│       ├── Controllers/
│       │   └── PersonsController.cs          # Endpoints HTTP para operações CRUD
│       ├── Data/
│       │   └── AppDbContext.cs               # Configuração do DbContext do EF Core
│       ├── DTOs/
│       │   ├── CreatePersonRequest.cs        # DTO para criar pessoa
│       │   ├── UpdatePersonRequest.cs        # DTO para atualizar pessoa
│       │   ├── PersonResponse.cs             # DTO de resposta da API
│       │   ├── AddressResponseDto.cs         # DTO de resposta de endereço
│       │   ├── CreateAddressDto.cs           # DTO para criar endereço
│       │   └── UpdateAddressDto.cs           # DTO para atualizar endereço
│       ├── Models/
│       │   ├── Person.cs                     # Modelo de entidade Person
│       │   └── PersonAddress.cs              # Modelo de entidade PersonAddress
│       ├── Repositories/
│       │   ├── IPersonRepository.cs          # Interface do repositório (contrato)
│       │   └── PersonRepository.cs           # Implementação do repositório (acesso a dados)
│       ├── Validators/
│       │   └── PersonValidator.cs            # Validação customizada para DateOfBirth
│       ├── Migrations/
│       │   └── [Arquivos de migration]       # Histórico de schema do banco de dados
│       ├── Program.cs                        # Startup e configuração da aplicação
│       ├── Api.csproj                        # Arquivo do projeto com referências NuGet
│       ├── appsettings.json                  # Configurações da aplicação
│       └── persons.db                        # Banco SQLite (desenvolvimento)
├── .gitignore
└── README.md                                  # Este arquivo
```

---

## 🚀 Como Começar

### Pré-requisitos

- **.NET 8 SDK** instalado ([Baixar](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server** 2019+ ou **SQL Server Express** ([Baixar](https://www.microsoft.com/pt-br/sql-server/sql-server-editions-express))
- **Visual Studio 2022** ou **VS Code** com extensão C#

### Instalação

#### 1. Clonar o Repositório

```bash
git clone https://github.com/filipeoliveira-ops/person-management-api.git
cd person-management-api
cd src/Api
```

#### 2. Restaurar Dependências

```bash
dotnet restore
```

#### 3. Configurar Conexão com o Banco de Dados

Edite `Program.cs` e configure a string de conexão:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer("Server=localhost;Database=PersonManagementApi;User Id=sa;Password=sua_senha;TrustServerCertificate=true;")
);
```

**Substitua:**
- `sua_senha` pela senha do usuário `sa` do SQL Server

#### 4. Criar o Banco de Dados e Aplicar Migrações

```bash
dotnet ef database update
```

Este comando:
- ✅ Cria o banco de dados `PersonManagementApi`
- ✅ Cria tabela `Persons`
- ✅ Cria tabela `PersonAddresses`
- ✅ Configura relacionamentos e constraints

#### 5. Executar a Aplicação

```bash
dotnet run
```

**Saída esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5164
      https://localhost:7087
```

#### 6. Acessar Swagger UI

Abra no navegador: **http://localhost:5164/swagger**

---

## 📚 Endpoints da API

### 1. Criar uma Nova Pessoa
**Requisição:**
```
POST /api/Persons
Content-Type: application/json
```

**Body:**
```json
{
  "name": "João Silva",
  "dateOfBirth": "1990-05-15",
  "address": {
    "street": "Rua das Flores",
    "number": "123",
    "complement": "Apt 45",
    "city": "São Paulo",
    "state": "SP",
    "country": "Brasil"
  }
}
```

**Resposta:** `201 Created`
```json
{
  "id": 1,
  "name": "João Silva",
  "dateOfBirth": "1990-05-15T00:00:00",
  "address": {
    "id": 1,
    "street": "Rua das Flores",
    "number": "123",
    "complement": "Apt 45",
    "city": "São Paulo",
    "state": "SP",
    "country": "Brasil"
  }
}
```

---

### 2. Listar Todas as Pessoas (com Paginação)
**Requisição:**
```
GET /api/Persons?page=1&pageSize=10&search=
```

**Resposta:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "João Silva",
    "dateOfBirth": "1990-05-15T00:00:00",
    "address": { /* objeto de endereço */ }
  }
]
```

**Parâmetros de Query:**
- `page` (opcional, padrão: 1) - Número da página
- `pageSize` (opcional, padrão: 10) - Itens por página
- `search` (opcional) - Buscar por nome, cidade ou estado

---

### 3. Obter Pessoa por ID
**Requisição:**
```
GET /api/Persons/{id}
```

**Resposta:** `200 OK`
```json
{
  "id": 1,
  "name": "João Silva",
  "dateOfBirth": "1990-05-15T00:00:00",
  "address": { /* objeto de endereço */ }
}
```

**Resposta de Erro:** `404 Not Found`
```json
{
  "message": "Pessoa não encontrada"
}
```

---

### 4. Atualizar uma Pessoa
**Requisição:**
```
PUT /api/Persons/{id}
Content-Type: application/json
```

**Body:**
```json
{
  "name": "João Silva Santos",
  "dateOfBirth": "1990-05-15",
  "address": {
    "street": "Rua Nova",
    "number": "456",
    "complement": "Apt 20",
    "city": "Rio de Janeiro",
    "state": "RJ",
    "country": "Brasil"
  }
}
```

**Resposta:** `200 OK`

---

### 5. Deletar uma Pessoa
**Requisição:**
```
DELETE /api/Persons/{id}
```

**Resposta:** `204 No Content` (sucesso, sem corpo)

**Resposta de Erro:** `404 Not Found`

---

## 🏛️ Arquitetura e Padrões de Design

### Repository Pattern
A aplicação utiliza o **Repository Pattern** para abstrair a lógica de acesso a dados:

```
Controller → IPersonRepository (interface) → PersonRepository (implementação) → DbContext → SQL Server
```

**Benefícios:**
- ✅ Separa lógica de negócio da lógica de acesso a dados
- ✅ Facilita testes (pode mockear o repositório)
- ✅ Mais fácil mudar provedores de banco (SQLite → SQL Server)
- ✅ Centraliza métodos de acesso a dados

### Data Transfer Objects (DTOs)
DTOs são usados em requisições/respostas da API:
- `CreatePersonRequest` - Body de requisição para POST
- `UpdatePersonRequest` - Body de requisição para PUT
- `PersonResponse` - Body de resposta para GET
- `AddressResponseDto` - Endereço aninhado na resposta

**Benefícios:**
- ✅ Desacopla contratos da API de modelos de banco de dados
- ✅ Validação ocorre na camada de API
- ✅ Segurança (nunca expõe todas as propriedades da entidade)

### Modelos de Entidade
- `Person` - Entidade principal com Id, Name, DateOfBirth, AddressId
- `PersonAddress` - Entidade de endereço com street, number, city, state, country
- **Relacionamento:** Um-para-Um (Person tem um Address)

### Injeção de Dependência
Todos os serviços são registrados em `Program.cs`:
```csharp
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
```

Quando um controller precisa de `IPersonRepository`, o framework automaticamente fornece uma instância de `PersonRepository`.

---

## ✅ Validações Implementadas

### FluentValidation (PersonValidator)
- **Name**: obrigatório + mínimo 3 caracteres
- **DateOfBirth**: não pode ser data futura

### Data Annotations (DTOs)
- **Name**: obrigatório + mínimo 3 caracteres
- **DateOfBirth**: obrigatório
- **Address**: obrigatório
- **Street, Number, City, State, Country**: obrigatórios

### Validação no Controller
- `DateOfBirth > DateTime.Now` → retorna HTTP 400 com ApiResponse de erro

---

## 📊 Schema do Banco de Dados

### Tabela Persons
| Coluna | Tipo | Restrições |
|--------|------|-----------|
| Id | INT | Chave Primária, Auto-incremento |
| Name | NVARCHAR(MAX) | NOT NULL |
| DateOfBirth | DATETIME | NOT NULL |
| AddressId | INT | Chave Estrangeira |

### Tabela PersonAddresses
| Coluna | Tipo | Restrições |
|--------|------|-----------|
| Id | INT | Chave Primária, Auto-incremento |
| Street | NVARCHAR(MAX) | NOT NULL |
| Number | NVARCHAR(MAX) | NOT NULL |
| Complement | NVARCHAR(MAX) | Nullable |
| City | NVARCHAR(MAX) | NOT NULL |
| State | NVARCHAR(MAX) | NOT NULL |
| Country | NVARCHAR(MAX) | NOT NULL |

**Relacionamentos:**
- Person → PersonAddress: Um-para-Um
- ON DELETE: Cascade (deletar uma Person também deleta seu Address)

---

## 📈 Histórico de Commits

Todos os commits seguem o formato: `type: description`

```
✅ chore: initialize Web API project with folder structure
✅ feat: create Person and PersonAddress models
✅ feat: create AppDbContext with EF Core configuration
✅ feat: implement repository pattern with PersonRepository
✅ feat: add DTOs and implement all CRUD endpoints
✅ feat: configure SQL Server and apply migrations
✅ feat: add FluentValidation for Person entity
✅ feat: add unit tests for PersonRepository (5 tests)
✅ feat: add ApiResponse wrapper and apply to all endpoints
✅ docs: update README with complete project documentation
```

---

## ✨ O Que Foi Concluído

### Fase 1: Setup do Projeto ✅
- [x] Modelos (Person, PersonAddress) com relacionamentos
- [x] Configuração do Entity Framework Core DbContext

### Fase 2: Camada de Acesso a Dados ✅
- [x] Repository Pattern (IPersonRepository, PersonRepository)
- [x] Migrações com SQL Server
- [x] CRUD completo com paginação e busca

### Fase 3: Camada de API ✅
- [x] 5 endpoints REST implementados
- [x] DTOs para requisição/resposta
- [x] Swagger configurado

### Fase 4: Qualidade e Boas Práticas ✅
- [x] FluentValidation para Person
- [x] ApiResponse padronizado em todos os endpoints
- [x] 5 testes unitários (xUnit + Moq + FluentAssertions)

---

## 📋 Melhorias Futuras

- **Frontend React** - Interface visual para consumir os endpoints da API
- **Docker** - Containerizar a aplicação junto com o SQL Server
- **CORS** - Configuração de Cross-Origin Resource Sharing para o frontend
- **Logging** - Integração com Serilog para rastreamento de erros em produção

---

## 🧪 Testando a API

### Usando Swagger UI (Recomendado)
1. Execute a aplicação: `dotnet run`
2. Abra no navegador: http://localhost:5164/swagger
3. Clique em qualquer endpoint
4. Clique em "Try it out"
5. Preencha o body da requisição
6. Clique em "Execute"

### Usando URL
```bash
# Criar uma pessoa
curl -X POST http://localhost:5164/api/Persons \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Maria Silva",
    "dateOfBirth": "1995-03-20",
    "address": {
      "street": "Rua ABC",
      "number": "789",
      "city": "Brasília",
      "state": "DF",
      "country": "Brasil"
    }
  }'

# Obter todas as pessoas
curl http://localhost:5164/api/Persons

# Obter pessoa por ID
curl http://localhost:5164/api/Persons/1

# Atualizar uma pessoa
curl -X PUT http://localhost:5164/api/Persons/1 \
  -H "Content-Type: application/json" \
  -d '{"name": "Maria Santos", ...}'

# Deletar uma pessoa
curl -X DELETE http://localhost:5164/api/Persons/1
```

---

## 🐛 Solução de Problemas

### Problema: "Não consegue conectar ao banco de dados"
**Solução:** 
- Verifique se SQL Server está rodando
- Confira a string de conexão em `Program.cs`
- Confirme as credenciais do banco (User Id, Password)
- Execute `dotnet ef database update` para criar o banco

### Problema: "DateOfBirth não pode ser no futuro"
**Solução:**
- Use uma data no passado para DateOfBirth
- Formato: YYYY-MM-DD

### Problema: "Pessoa não encontrada (404)"
**Solução:**
- Verifique se o ID existe no banco de dados
- Consulte SQL Server Management Studio

---

## 📚 Conceitos-Chave Aprendidos

1. **Repository Pattern** - Abstração de acesso a dados
2. **Injeção de Dependência** - Contenedor de DI do ASP.NET Core
3. **Entity Framework Core** - ORM para operações de banco de dados
4. **DTOs** - Desacoplamento de contratos de API de entidades
5. **Async/Await** - Operações de banco de dados não-bloqueantes
6. **Design de API RESTful** - Métodos HTTP, status codes, nomenclatura de recursos
7. **Validações** - Data annotations e validadores customizados
8. **Database Migrations** - Controle de versão para mudanças de schema
9. **Swagger/OpenAPI** - Documentação e testes de API

---


## 📄 Licença

Este projeto é para fins educacionais e de avaliação.

---

**Última Atualização:** 31 de Maio de 2026  
**Status:** 🟢 Concluído