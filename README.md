# API de Gerenciamento de Pessoas 

**Status:** 🟢 Concluído  
**Última Atualização:** 13 de Setembro de 2026  
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
| **xUnit + FluentAssertions** | 2.9.3 / 8.10.0 | Testes automatizados |
| **EF Core Sqlite** | 8.0 | Banco em memória usado nos testes |

---

## 🏗️ Estrutura do Projeto

```
person-management-api/
├── src/
│   └── Api/
│       ├── Controllers/
│       │   └── PersonController.cs           # Endpoints HTTP (classe PersonsController)
│       ├── Data/
│       │   └── AppDbContext.cs               # DbContext e configuração do relacionamento 1:1
│       ├── DTOs/
│       │   ├── CreatePersonRequest.cs        # DTO de POST (contém também CreateAddressDto)
│       │   ├── UpdatePersonRequest.cs        # DTO de PUT (contém também UpdateAddressDto)
│       │   └── PersonResponse.cs             # DTO de resposta (contém também AddressResponseDto)
│       ├── Models/
│       │   ├── Person.cs                     # Entidade Person (principal do relacionamento)
│       │   └── PersonAddress.cs              # Entidade PersonAddress (dependente, guarda a FK)
│       ├── Repositories/
│       │   ├── IPersonRepository.cs          # Interface do repositório (contrato)
│       │   └── PersonRepository.cs           # Implementação do repositório (acesso a dados)
│       ├── Response/
│       │   └── ApiResponse.cs                # Wrapper padrão de resposta da API
│       ├── Validators/
│       │   └── PersonValidator.cs            # Regras FluentValidation para Person
│       ├── Migrations/
│       │   └── [Arquivos de migration]       # Histórico de schema do banco de dados
│       ├── Properties/
│       │   └── launchSettings.json           # Perfis de execução (portas e ambiente)
│       ├── Program.cs                        # Startup e configuração da aplicação
│       ├── Api.csproj                        # Arquivo do projeto com referências NuGet
│       ├── appsettings.json                  # Configurações da aplicação
│       └── appsettings.Development.json      # Connection string de desenvolvimento
├── tests/
│   └── Api.Tests/
│       ├── PersonRepositoryTests.cs          # Testes do PersonRepository (SQLite in-memory)
│       └── Api.Tests.csproj                  # Projeto de testes
├── .vscode/                                  # launch.json e tasks.json (debug no VS Code)
├── PersonManagement.sln                      # Solução (necessária para F5 no Visual Studio)
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
git clone https://github.com/ooliveira-ops/person-management-api.git
cd person-management-api
```

#### 2. Restaurar Dependências

```bash
dotnet restore
```

#### 3. Configurar Conexão com o Banco de Dados

⚠️ **Nunca coloque a connection string dentro do `Program.cs`.** O projeto já teve uma senha
de SQL Server exposta no histórico do Git por causa disso. Desde o commit
`29771df`, o `Program.cs` apenas lê a configuração:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
```

**Configuração padrão (autenticação Windows — sem senha):**

O `src/Api/appsettings.Development.json` já vem com uma connection string que usa
`Trusted_Connection=True`, ou seja, autentica pelo usuário do Windows e não guarda
segredo nenhum:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PersonManagementApi;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Ajuste o `Server=` se a sua instância tiver outro nome.

**Se você precisar usar usuário e senha** (login SQL, container, servidor remoto),
**não** edite o `appsettings.Development.json` — ele é versionado. Use o gerenciador
de segredos do .NET, que grava fora do repositório:

```bash
cd src/Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=PersonManagementApi;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=true;"
```

Alternativa por variável de ambiente (útil em CI/containers):

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=...;Password=...;"
```

O ASP.NET Core resolve a configuração nesta ordem — user-secrets e variáveis de
ambiente sobrescrevem o appsettings, sem exigir mudança de código.

#### 4. Criar o Banco de Dados e Aplicar Migrações

```bash
dotnet ef database update --project src/Api
```

Este comando:
- ✅ Cria o banco de dados `PersonManagementApi`
- ✅ Cria tabela `Persons`
- ✅ Cria tabela `PersonAddresses`
- ✅ Configura relacionamentos e constraints

#### 5. Executar a Aplicação

**Opção A — linha de comando:**

```bash
dotnet run --project src/Api
```

**Saída esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5164
      https://localhost:7221
```

**Opção B — Visual Studio (com debug):**

1. Abra o **`PersonManagement.sln`** na raiz (não use "Abrir Pasta" — veja o porquê abaixo)
2. Defina **`Api`** como projeto de inicialização
3. Escolha o perfil **`http`** ou **`https`** no dropdown ao lado do botão de play
4. **F5**

**Opção C — VS Code (com debug):**

Pressione **F5** e escolha o perfil **"Debug API (http)"**. Ele já define
`ASPNETCORE_ENVIRONMENT=Development` e `ASPNETCORE_URLS=http://localhost:5164`.
Há também o perfil **"Attach to process (dotnet run)"** para anexar o debugger a
uma aplicação já em execução.

> **Por que a solução importa:** abrindo o repositório em modo "abrir pasta", sem
> um `.sln`, o Visual Studio **ignora o `launchSettings.json`**. A aplicação sobe
> em `Production` (o default do .NET quando `ASPNETCORE_ENVIRONMENT` não é
> definido) e na porta `5000` (default do Kestrel). Como o Swagger e o redirect da
> raiz estão dentro de `if (app.Environment.IsDevelopment())` no `Program.cs`,
> nenhum dos dois é registrado — e `localhost:5000/` devolve **404**.

<details>
<summary>Como a solução foi criada (para quem precisar recriá-la)</summary>

O SDK 10.x gera `.slnx` por padrão. Para obter o formato clássico `.sln`,
compatível com o Visual Studio 2022, é preciso a flag `--format sln`:

```bash
dotnet new sln -n PersonManagement --format sln
dotnet sln add src/Api/Api.csproj
dotnet sln add tests/Api.Tests/Api.Tests.csproj
```
</details>

#### 6. Acessar Swagger UI

Abra no navegador: **http://localhost:5164/swagger**

> A raiz (`/`) redireciona para o Swagger — mas só no ambiente **Development**.

#### Portas e perfis

| Perfil | Portas | Observação |
|--------|--------|-----------|
| `http` | `http://localhost:5164` | Loga `warn: Failed to determine the https port for redirect`. É inofensivo: sem porta HTTPS conhecida, o `UseHttpsRedirection` não redireciona e o HTTP passa normalmente. |
| `https` | `https://localhost:7221` + `http://localhost:5164` | Com HTTPS disponível, a 5164 passa a devolver **307** redirecionando para a 7221. Não é erro — é o `UseHttpsRedirection` funcionando. |

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

> O campo `complement` é **opcional** — pode ser omitido ou enviado como `null`.

**Resposta:** `201 Created`

Todas as respostas são embrulhadas no wrapper `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Person created successfully",
  "data": {
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
{
  "success": true,
  "message": "Operation successful",
  "data": [
    {
      "id": 1,
      "name": "João Silva",
      "dateOfBirth": "1990-05-15T00:00:00",
      "address": { /* objeto de endereço */ }
    }
  ]
}
```

**Parâmetros de Query:**
- `page` (opcional, padrão: 1) - Número da página. Valores `< 1` são normalizados para `1`.
- `pageSize` (opcional, padrão: 10) - Itens por página. Limitado ao intervalo **1 a 100**.
- `search` (opcional) - Buscar por nome, cidade ou estado

> A normalização da paginação acontece no `PersonRepository`. Sem ela, `page=0`
> geraria `Skip(-10)` e o SQL Server rejeitaria a consulta com erro 500.

---

### 3. Obter Pessoa por ID
**Requisição:**
```
GET /api/Persons/{id}
```

**Resposta:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "name": "João Silva",
    "dateOfBirth": "1990-05-15T00:00:00",
    "address": { /* objeto de endereço, ou null se a pessoa não tiver endereço */ }
  }
}
```

**Resposta de Erro:** `404 Not Found`
```json
{
  "success": false,
  "message": "Person not found",
  "data": null
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

**Resposta de Erro:** `400 Bad Request` — mesmas regras do POST (nome com menos de
3 caracteres, data de nascimento no futuro)
```json
{
  "success": false,
  "message": "DateOfBirth cannot be in the future",
  "data": null
}
```

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
- ✅ Facilita testes: o controller pode ser testado com um mock de `IPersonRepository`,
     enquanto o `PersonRepository` é testado contra um banco real
- ✅ Mais fácil mudar provedores de banco (é o que permite testar em SQLite e rodar em SQL Server)
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
- `Person` - Entidade **principal**, com Id, Name, DateOfBirth e a navegação `Address`
- `PersonAddress` - Entidade **dependente**, com Id, `PersonId` (FK), Street, Number,
  Complement, City, State, Country
- **Relacionamento:** Um-para-Um. A chave estrangeira fica em `PersonAddress`, porque
  é o endereço que depende da pessoa — não o contrário. Veja
  [Schema do Banco de Dados](#-schema-do-banco-de-dados).

### Injeção de Dependência
Todos os serviços são registrados em `Program.cs`:
```csharp
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
```

Quando um controller precisa de `IPersonRepository`, o framework automaticamente fornece uma instância de `PersonRepository`.

---

## ✅ Validações Implementadas

### Data Annotations (DTOs)
Aplicadas automaticamente pelo `[ApiController]`, que devolve **400** antes de o
método do controller executar.

- **Name**: obrigatório + mínimo 3 caracteres
- **DateOfBirth**: obrigatório
- **Address**: obrigatório
- **Street, Number, City, State, Country**: obrigatórios
- **Complement**: opcional (nullable no DTO **e** no banco)

### FluentValidation (PersonValidator)
Regras de negócio sobre a entidade `Person`, aplicadas **tanto no POST quanto no
PUT**. O `PersonsController` injeta `IValidator<Person>` e roda as regras antes de
gravar; se alguma falhar, devolve **400** com as mensagens agregadas no `ApiResponse`.

- **Name**: obrigatório + mínimo 3 caracteres
- **DateOfBirth**: não pode ser data futura

O validador é a **fonte única** dessas regras: mudar algo em `PersonValidator.cs`
passa a valer nos dois endpoints automaticamente. No `PUT`, a validação roda antes
de a entidade rastreada pelo EF ser alterada, para que uma requisição inválida não
deixe mudanças pendentes no `DbContext`.

> Antes da centralização, a checagem de data futura era feita à mão dentro do
> `CreatePerson` e o `PUT` aceitava data de nascimento no futuro.

---

## 📊 Schema do Banco de Dados

### Tabela Persons
| Coluna | Tipo | Restrições |
|--------|------|-----------|
| Id | INT | Chave Primária, Auto-incremento |
| Name | NVARCHAR(MAX) | NOT NULL |
| DateOfBirth | DATETIME2 | NOT NULL |

### Tabela PersonAddresses
| Coluna | Tipo | Restrições |
|--------|------|-----------|
| Id | INT | Chave Primária, Auto-incremento |
| PersonId | INT | **Chave Estrangeira → Persons.Id**, índice único |
| Street | NVARCHAR(MAX) | NOT NULL |
| Number | NVARCHAR(MAX) | NOT NULL |
| Complement | NVARCHAR(MAX) | **NULL** (opcional) |
| City | NVARCHAR(MAX) | NOT NULL |
| State | NVARCHAR(MAX) | NOT NULL |
| Country | NVARCHAR(MAX) | NOT NULL |

**Relacionamentos:**
- Person → PersonAddress: Um-para-Um
- A **FK fica em `PersonAddresses.PersonId`**: o endereço é o lado dependente, a
  pessoa é o principal.
- **ON DELETE Cascade**: deletar uma `Person` deleta o `PersonAddress` dela.
  Deletar um endereço **não** afeta a pessoa.

> **Por que isso importa:** até a migration `FixPersonAddressCascadeDelete`, a FK
> estava invertida (`Persons.AddressId` → `PersonAddresses.Id`). Na prática, o
> endereço era o principal e o cascade corria ao contrário: apagar um endereço
> apagava a pessoa. Também era impossível cadastrar uma pessoa sem endereço, já
> que `AddressId` era obrigatório.

**Migrations aplicadas:**

| Migration | O que faz |
|-----------|-----------|
| `InitialCreate` | Cria as tabelas `Persons` e `PersonAddresses` |
| `MakeComplementNullable` | Torna `Complement` nullable, alinhando banco e DTO |
| `FixPersonAddressCascadeDelete` | Move a FK para `PersonAddresses.PersonId` e inverte o cascade |

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
✅ security: remover senha do banco do codigo-fonte
✅ test: substituir mocks por testes reais com SQLite in-memory
✅ fix: alinhar nullability de Complement entre DTO e banco
✅ fix: inverter cascade delete (FK movida para PersonAddress)
✅ fix: validar paginacao (page minimo 1, pageSize com teto)
✅ fix: tratar Address nulo no MapToResponse e no UpdatePerson
✅ fix: centralizar validacao de data futura no PersonValidator
✅ chore: ignorar bancos locais e arquivos de segredo
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
- [x] 5 testes do `PersonRepository` (xUnit + FluentAssertions + SQLite in-memory)

### Fase 5: Correções da Code Review ✅
- [x] Testes reescritos para exercitar o `PersonRepository` real
- [x] `Complement` nullable no banco, alinhado ao DTO
- [x] Cascade delete corrigido (FK movida para `PersonAddress`)
- [x] Paginação validada (`page` mínimo 1, `pageSize` limitado a 100)
- [x] `Address` nulo tratado no `MapToResponse` e no `UpdatePerson`
- [x] Validação de data futura centralizada no `PersonValidator` (vale no POST e no PUT)
- [x] Connection string fora do código-fonte
- [x] `.gitignore` ajustado (bancos locais e arquivos de segredo)

---

## 🧪 Estratégia de Testes

```bash
dotnet test
```

Os testes instanciam o **`PersonRepository` real** contra um banco **SQLite
in-memory**, criado do zero a cada teste:

```csharp
var conexao = new SqliteConnection("DataSource=:memory:");
conexao.Open();   // o banco só existe enquanto houver conexão aberta
var context = new AppDbContext(options);
context.Database.EnsureCreated();
```

Não há mock do repositório. Um mock só confirmaria que o próprio mock respondeu o
que foi configurado — a versão anterior destes testes tinha esse problema e passava
mesmo com o código de produção quebrado.

**Limites conhecidos desta abordagem:**

- **SQLite ≠ SQL Server.** `string.Contains` vira `instr()` (case-sensitive) no
  SQLite e `LIKE` (case-insensitive) no SQL Server. A busca pode se comportar
  diferente em produção.
- **As migrations não são exercitadas.** `EnsureCreated()` gera o schema direto do
  modelo C# e pula a pasta `Migrations/`. Validar as migrations exige um SQL Server
  de verdade.
- **Só a camada de repositório tem cobertura.** Não há testes para o
  `PersonsController`, para a validação dos DTOs nem para o pipeline HTTP.

---

## 📋 Melhorias Futuras

- **Frontend React** - Interface visual para consumir os endpoints da API
- **Docker** - Containerizar a aplicação junto com o SQL Server
- **CORS** - Configuração de Cross-Origin Resource Sharing para o frontend
- **Logging** - Integração com Serilog para rastreamento de erros em produção

---

## 🧪 Testando a API

### Usando Swagger UI (Recomendado)
1. Execute a aplicação: `dotnet run --project src/Api`
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

### Usando Postman
- A variável `{{URL}}` da collection precisa do **esquema correto**: `https://` para
  a porta **7221**, `http://` para a **5164**. Apontar `http://localhost:7221`
  resulta em `ECONNREFUSED`.
- Para HTTPS, desligue *Settings → General → SSL certificate verification* e/ou
  rode `dotnet dev-certs https --trust`.

---

## 🐛 Solução de Problemas

### Problema: "Não consegue conectar ao banco de dados"
**Solução:** 
- Verifique se SQL Server está rodando
- Confira a connection string em `src/Api/appsettings.Development.json`
  (**não** em `Program.cs` — ela saiu do código no commit `29771df`)
- Se estiver usando user-secrets, confira com `dotnet user-secrets list`
- Execute `dotnet ef database update --project src/Api` para criar o banco

### Problema: "DateOfBirth não pode ser no futuro"
**Solução:**
- Use uma data no passado para DateOfBirth
- Formato: YYYY-MM-DD
- Vale para **POST e PUT**: a regra vem do `PersonValidator` e é aplicada nos dois

### Problema: "Pessoa não encontrada (404)"
**Solução:**
- Verifique se o ID existe no banco de dados
- Consulte SQL Server Management Studio

### Problema: Swagger devolve 404 e a aplicação subiu na porta 5000
**Causa:** a aplicação está rodando em `Production`. O Swagger e o redirect da raiz
só são registrados dentro de `if (app.Environment.IsDevelopment())`.

Confirme na primeira linha do log: `Hosting environment: Production`.

**Solução:**
- Abra o **`PersonManagement.sln`** no Visual Studio em vez de usar "Abrir Pasta" —
  sem a solução, o `launchSettings.json` é ignorado e nem o ambiente nem a porta
  são aplicados
- Ou defina a variável manualmente:
  ```bash
  $env:ASPNETCORE_ENVIRONMENT = "Development"
  dotnet run --project src/Api
  ```

### Problema: `warn: Failed to determine the https port for redirect`
**Não é erro.** Acontece no perfil `http`, que não expõe porta HTTPS. Sem uma porta
de destino conhecida, o `UseHttpsRedirection` simplesmente não redireciona e as
requisições HTTP seguem normalmente. Para eliminar o aviso, use o perfil `https`.

### Problema: a porta 5164 devolve 307 em vez da resposta
**Não é erro.** No perfil `https`, o `UseHttpsRedirection` redireciona a 5164 para a
7221. Clientes que seguem redirects (navegador, Postman com *Automatically follow
redirects* ligado) funcionam normalmente. Para evitar o salto, chame direto
`https://localhost:7221`.

### Problema: `ECONNREFUSED` no Postman
**Causa:** não há nada escutando naquela porta. **Não** é problema de certificado.

**Solução:**
- Confirme que a aplicação está rodando e em qual perfil
- Verifique o **esquema** da variável `{{URL}}` da collection: `https://` para a
  **7221**, `http://` para a **5164**. `http://localhost:7221` sempre falha.

### Problema: erro de certificado SSL no Postman
**Solução:**
- Desligue *Settings → General → SSL certificate verification*, e/ou
- Confie no certificado de desenvolvimento: `dotnet dev-certs https --trust`

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

**Última Atualização:** 13 de Setembro de 2026  
**Status:** 🟢 Concluído
