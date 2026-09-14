# API de Gerenciamento de Pessoas

**Status:** 🟢 Concluído  
**Data de Entrega:** 2 de Junho de 2026  
**Revisão Técnica:** 10 de Setembro de 2026 — Tech Lead, melhorias aplicadas  
**Última Atualização:** 14 de Setembro de 2026

---

## 📋 Visão Geral

API Web RESTful em **ASP.NET Core 8** para gerenciar pessoas e seus endereços — CRUD
completo com paginação, busca e validação, sobre SQL Server.

O projeto é uma avaliação técnica: o foco está em organização de código, decisões de
modelagem e confiabilidade dos testes, não em volume de funcionalidades. Ele **não**
tem autenticação, autorização nem interface visual.

Entregue em **2 de junho de 2026** e revisado em code review pelo **Tech Lead** em
**10 de setembro de 2026**. A revisão apontou sete problemas — em segurança,
modelagem de dados e confiabilidade dos testes —, todos corrigidos, cada um com um
teste de regressão que falha se a correção for revertida. As decisões que vieram
dessa revisão estão documentadas ao longo deste README, nos pontos onde importam.

---

## 🛠️ Stack Tecnológico

| Tecnologia | Versão | Propósito |
|------------|--------|----------|
| **.NET / ASP.NET Core** | 8.0 | Framework e Web API |
| **Entity Framework Core** | 8.0 | ORM |
| **SQL Server** | 2019+ | Banco de dados |
| **FluentValidation** | 12.1.1 | Regras de negócio |
| **Swagger/OpenAPI** | 6.6.2 | Documentação e testes manuais |
| **xUnit + FluentAssertions** | 2.9.3 / 8.10.0 | Testes |
| **Moq** | 4.20.72 | Mock do repositório nos testes de controller |
| **EF Core Sqlite** | 8.0 | Banco em memória nos testes unitários |

---

## 🏗️ Estrutura do Projeto

```
person-management-api/
├── src/Api/
│   ├── Controllers/PersonController.cs     # Endpoints (classe PersonsController)
│   ├── Data/AppDbContext.cs                # DbContext e o relacionamento 1:1
│   ├── DTOs/                               # Contratos de entrada e saída
│   ├── Models/                             # Person e PersonAddress
│   ├── Repositories/                       # IPersonRepository e implementação
│   ├── Response/ApiResponse.cs             # Envelope padrão das respostas
│   ├── Validators/PersonValidator.cs       # Regras de negócio
│   ├── Migrations/                         # Histórico de schema
│   ├── Properties/launchSettings.json      # Perfis e portas
│   ├── Program.cs                          # Startup, DI e pipeline
│   └── appsettings*.json                   # Configuração
├── tests/Api.Tests/
│   ├── PersonRepositoryTests.cs            # Unitários (SQLite in-memory)
│   ├── PersonRepositoryIntegrationTests.cs # Integração (SQL Server real)
│   └── PersonsControllerTests.cs           # Controller (repositório mockado)
├── PersonManagement.sln                    # Necessária para F5 no Visual Studio
└── README.md
```

---

## 🚀 Como Começar

### Pré-requisitos

- **.NET 8 SDK** ([Baixar](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server** 2019+ ou **SQL Server Express**
- **Visual Studio 2022** ou **VS Code** com extensão C#

### 1. Clonar e restaurar

```bash
git clone https://github.com/ooliveira-ops/person-management-api.git
cd person-management-api
dotnet restore
```

### 2. Configurar a conexão com o banco

⚠️ **Nunca coloque a connection string dentro do `Program.cs`.** O projeto já teve
uma senha de SQL Server exposta no histórico do Git por causa disso. Desde o commit
`29771df`, o `Program.cs` apenas lê a configuração.

**Padrão (autenticação Windows, sem senha):** o `src/Api/appsettings.Development.json`
já traz uma connection string com `Trusted_Connection=True`, que autentica pelo
usuário do Windows e não guarda segredo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR\\SUA_INSTANCIA;Database=PersonManagementApi;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Ajuste o `Server=` para a sua instância — o valor versionado aponta para uma
instância local do SQL Server Express.

**Se precisar de usuário e senha** (login SQL, container, servidor remoto), **não**
edite o `appsettings.Development.json` — ele é versionado. Use o gerenciador de
segredos do .NET, que grava fora do repositório:

```bash
cd src/Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=SEU_SERVIDOR\SUA_INSTANCIA;Database=PersonManagementApi;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=true;"
```

User-secrets e variáveis de ambiente sobrescrevem o appsettings sem exigir mudança
de código.

### 3. Criar o banco

```bash
dotnet ef database update --project src/Api
```

### 4. Executar

**Linha de comando:**

```bash
dotnet run --project src/Api --launch-profile http
```

**Visual Studio (com debug):** abra o **`PersonManagement.sln`** — não use "Abrir
Pasta" —, defina `Api` como projeto de inicialização, escolha o perfil `http` ou
`https` e pressione **F5**.

> **Por que a solução importa:** sem um `.sln`, o Visual Studio ignora o
> `launchSettings.json`. A aplicação sobe em `Production` (o default quando
> `ASPNETCORE_ENVIRONMENT` não é definido) na porta `5000`, e como o Swagger está
> dentro de `if (app.Environment.IsDevelopment())`, `localhost:5000/` devolve **404**.

### 5. Acessar o Swagger

**http://localhost:5164/swagger** — a raiz (`/`) redireciona para lá, só em Development.

### Portas e perfis

| Perfil | Portas | Observação |
|--------|--------|-----------|
| `http` | `http://localhost:5164` | Loga `warn: Failed to determine the https port for redirect`. Inofensivo: sem porta HTTPS conhecida, o `UseHttpsRedirection` não redireciona. |
| `https` | `https://localhost:7221` + `http://localhost:5164` | A 5164 devolve **307** redirecionando para a 7221. Não é erro — é o middleware funcionando. |

---

## 📚 Endpoints da API

Todas as respostas usam o envelope `ApiResponse<T>`:

```json
{ "success": true, "message": "Operation successful", "data": {} }
```

Em erro, `success` é `false`, `message` traz o motivo e `data` vem `null`.

| Verbo | Rota | Sucesso | Erros |
|---|---|---|---|
| POST | `/api/Persons` | `201 Created` | `400` validação |
| GET | `/api/Persons` | `200 OK` | — |
| GET | `/api/Persons/{id}` | `200 OK` | `404` não encontrada |
| PUT | `/api/Persons/{id}` | `200 OK` | `400` validação, `404` não encontrada |
| DELETE | `/api/Persons/{id}` | `204 No Content` | `404` não encontrada |

### Body de criação e atualização

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

O campo `complement` é **opcional** — pode ser omitido ou enviado como `null`.

### Resposta completa (exemplo do POST)

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

No GET por id, `address` vem `null` se a pessoa não tiver endereço cadastrado.

### Listagem — parâmetros de query

- `page` (padrão: 1) — valores `< 1` são normalizados para `1`
- `pageSize` (padrão: 10) — limitado ao intervalo **1 a 100**
- `search` — busca por nome, cidade ou estado

> A normalização acontece no `PersonRepository`, não no controller, para valer em
> qualquer chamador. Sem ela, `page=0` geraria `Skip(-10)` e o SQL Server rejeitaria
> a consulta com erro 500.

---

## 🏛️ Arquitetura e Decisões

### Repository Pattern

```
Controller → IPersonRepository → PersonRepository → DbContext → SQL Server
```

O controller não conhece o banco. Além de separar responsabilidades, é isso que
permite testar cada camada com a ferramenta certa: o repositório contra um banco
real, o controller contra um mock da interface.

### Modelos e o relacionamento 1:1

- `Person` — lado **principal**: Id, Name, DateOfBirth e a navegação `Address`
- `PersonAddress` — lado **dependente**: Id, `PersonId` (FK), Street, Number,
  Complement, City, State, Country

A chave estrangeira fica em `PersonAddress` porque no EF Core quem carrega a FK é o
dependente — e um endereço só existe se houver uma pessoa. É essa escolha que faz o
cascade correr na direção certa.

### CORS

Navegadores bloqueiam uma página de ler respostas de outra origem. As origens
autorizadas ficam em `Cors:AllowedOrigins`, no `appsettings.Development.json`:

```json
"Cors": {
  "AllowedOrigins": [ "http://localhost:3000", "http://localhost:5173" ]
}
```

Para liberar um frontend novo, adicione a URL dele nessa lista — sem recompilar.

Duas coisas que costumam confundir: **CORS não protege a API** (curl, Postman e
chamadas de servidor ignoram a regra; só navegadores a aplicam), e a API **responde
normalmente** mesmo a uma origem não autorizada. O que falta na resposta é o header
`Access-Control-Allow-Origin` — e é o navegador que recusa a leitura por causa disso.

Para verificar sem um frontend:

```bash
curl -i -k -H "Origin: http://localhost:3000" https://localhost:7221/api/Persons
```

O header `Access-Control-Allow-Origin` deve aparecer para origem autorizada e sumir
para qualquer outra.

### Logging

Usa o `ILogger<T>` nativo do ASP.NET Core, injetado no `PersonsController`. Os logs
cobrem os caminhos que antes sumiam em silêncio: busca que resulta em 404, validação
recusada, criação e remoção bem-sucedidas.

As mensagens usam **templates com placeholders nomeados**, não interpolação:

```csharp
_logger.LogWarning("Pessoa {PersonId} não encontrada", id);
```

A diferença não é cosmética: com placeholder, o logger recebe o template e o valor
como campos separados, o que mantém `PersonId` pesquisável quando os logs vão para
arquivo ou ferramenta de busca. Com `$"..."`, tudo vira texto opaco.

Os níveis ficam em `Logging.LogLevel`, no `appsettings.json`. A categoria de cada log
é o tipo genérico (`Api.Controllers.PersonsController`), e é por ela que se filtra.

---

## ✅ Validações

### Data Annotations (DTOs)

Aplicadas pelo `[ApiController]`, que devolve **400** antes de o método executar.

- **Name**: obrigatório, mínimo 3 caracteres
- **DateOfBirth**, **Address**: obrigatórios
- **Street, Number, City, State, Country**: obrigatórios
- **Complement**: opcional, nullable no DTO **e** no banco

### FluentValidation (PersonValidator)

Regras de negócio sobre a entidade, aplicadas **no POST e no PUT**. O controller
injeta `IValidator<Person>` e roda as regras antes de gravar; se alguma falhar,
devolve 400 com as mensagens agregadas.

- **Name**: obrigatório, mínimo 3 caracteres
- **DateOfBirth**: não pode ser data futura

O validador é a **fonte única** dessas regras — mudar `PersonValidator.cs` vale nos
dois endpoints. No PUT, a validação roda antes de a entidade rastreada pelo EF ser
alterada, para que uma requisição inválida não deixe mudanças pendentes no contexto.

---

## 📊 Schema do Banco de Dados

### Persons
| Coluna | Tipo | Restrições |
|--------|------|-----------|
| Id | INT | PK, auto-incremento |
| Name | NVARCHAR(MAX) | NOT NULL |
| DateOfBirth | DATETIME2 | NOT NULL |

### PersonAddresses
| Coluna | Tipo | Restrições |
|--------|------|-----------|
| Id | INT | PK, auto-incremento |
| PersonId | INT | **FK → Persons.Id**, índice único |
| Street, Number, City, State, Country | NVARCHAR(MAX) | NOT NULL |
| Complement | NVARCHAR(MAX) | **NULL** (opcional) |

**Relacionamento:** um-para-um, com a FK em `PersonAddresses.PersonId`.
**ON DELETE Cascade:** apagar uma `Person` apaga o endereço dela; apagar um endereço
**não** afeta a pessoa.

> **Por que isso importa:** até a migration `FixPersonAddressCascadeDelete` a FK
> estava invertida (`Persons.AddressId`). O endereço era o principal e o cascade
> corria ao contrário — apagar um endereço apagava a pessoa. Também era impossível
> cadastrar alguém sem endereço, já que `AddressId` era obrigatório.

**Migrations:**

| Migration | O que faz |
|-----------|-----------|
| `InitialCreate` | Cria `Persons` e `PersonAddresses` |
| `MakeComplementNullable` | Torna `Complement` nullable, alinhando banco e DTO |
| `FixPersonAddressCascadeDelete` | Move a FK para `PersonAddresses.PersonId` e inverte o cascade |

---

## 🧪 Estratégia de Testes

São **10 testes em três camadas**, cada uma provando uma coisa diferente:

| Arquivo | Testes | Banco | O que prova |
|---|---|---|---|
| `PersonRepositoryTests` | 6 | SQLite in-memory | Lógica do repositório: CRUD, busca, teto de `pageSize` |
| `PersonRepositoryIntegrationTests` | 2 | SQL Server real | Migrations aplicando do zero, cascade real, `page=0` |
| `PersonsControllerTests` | 2 | nenhum (mock) | Status codes, `Address` nulo, validação de data |

### Rodando

```bash
dotnet test                                                  # tudo
dotnet test --filter "FullyQualifiedName~IntegrationTests"   # só integração
```

### ⚠️ Configuração obrigatória para os testes de integração

Eles precisam de um SQL Server acessível e de uma connection string própria. **Sem
isso, 2 testes falham** com `InvalidOperationException`.

```bash
cd tests/Api.Tests
dotnet user-secrets set "ConnectionStrings:TestDatabase" "Server=SEU_SERVIDOR\SUA_INSTANCIA;Database=PersonManagementApi_Tests;Trusted_Connection=True;TrustServerCertificate=True;"
```

Alternativa por variável de ambiente:

```powershell
$env:ConnectionStrings__TestDatabase = "Server=SEU_SERVIDOR\SUA_INSTANCIA;Database=PersonManagementApi_Tests;Trusted_Connection=True;TrustServerCertificate=True;"
```

Dois cuidados que o próprio código impõe:

- **O nome do banco precisa terminar em `_Tests`.** Cada teste chama
  `EnsureDeleted()`, que apaga o banco inteiro antes de recriá-lo — apontar para o
  banco de desenvolvimento destruiria seus dados. Há uma verificação que recusa
  qualquer outro nome.
- **Não há valor padrão no código.** Um default traria de volta a connection string
  versionada, que é justamente o problema que a review apontou.

### Por que duas camadas de banco

Os testes unitários usam SQLite porque são rápidos e isolados. Mas `EnsureCreated()`
gera o schema direto do modelo C# e **ignora a pasta `Migrations/`** — então nada ali
valida as migrations. E os dialetos divergem: `string.Contains` vira `instr()`
(case-sensitive) no SQLite e `LIKE` (case-insensitive) no SQL Server.

O caso mais instrutivo foi o `page=0`: o SQLite trata `OFFSET` negativo como zero, e
o teste passava mesmo sem a correção aplicada. Só contra SQL Server ele tem força.

### O que os testes não cobrem

- **Pipeline HTTP completo** — model binding, serialização e o 400 automático do
  `[ApiController]` não são exercitados; os testes de controller chamam o método
  diretamente.
- **CORS e logging** — nenhum teste automatizado; a verificação é manual, conforme
  descrito nas seções acima.

---

## 🧪 Testando a API manualmente

**Swagger UI** (recomendado): rode a aplicação e abra
http://localhost:5164/swagger — use "Try it out" em qualquer endpoint.

**Postman:** a variável `{{URL}}` precisa do esquema correto — `https://` para a
porta **7221**, `http://` para a **5164**. Apontar `http://localhost:7221` resulta em
`ECONNREFUSED`. Para HTTPS, desligue *Settings → General → SSL certificate
verification* ou rode `dotnet dev-certs https --trust`.

---

## 🐛 Solução de Problemas

### 2 testes falham com "Connection string 'TestDatabase' não configurada"
São os testes de integração. Configure o user-secrets conforme a seção
[Estratégia de Testes](#-estratégia-de-testes). Para rodar só os unitários enquanto
isso: `dotnet test --filter "FullyQualifiedName!~IntegrationTests"`.

### Não consegue conectar ao banco
- Verifique se o SQL Server está rodando
- Confira a connection string em `src/Api/appsettings.Development.json` — **não** em
  `Program.cs`, de onde ela saiu no commit `29771df`
- Com user-secrets, confira via `dotnet user-secrets list`
- Rode `dotnet ef database update --project src/Api`

### Swagger devolve 404 e a aplicação subiu na porta 5000
A aplicação está em `Production` — confirme na primeira linha do log
(`Hosting environment:`). Swagger e o redirect da raiz só existem em Development.
Abra o `PersonManagement.sln` em vez de "Abrir Pasta", ou defina
`$env:ASPNETCORE_ENVIRONMENT = "Development"` antes do `dotnet run`.

### `warn: Failed to determine the https port for redirect`
Não é erro. Acontece no perfil `http`, que não expõe porta HTTPS: sem destino
conhecido, o `UseHttpsRedirection` não redireciona e o HTTP passa normalmente.

### A porta 5164 devolve 307
Não é erro. No perfil `https`, o `UseHttpsRedirection` redireciona para a 7221. Para
evitar o salto, chame direto `https://localhost:7221`.

### `ECONNREFUSED`
Nada escutando na porta — aplicação parada ou perfil errado. **Não** é problema de
certificado.

### O frontend recebe erro de CORS
Adicione a origem dele em `Cors:AllowedOrigins`, no `appsettings.Development.json`.
Lembre que a porta faz parte da origem: `localhost:3000` e `localhost:5173` são
origens diferentes.

### "DateOfBirth não pode ser no futuro"
Use data no passado, formato `YYYY-MM-DD`. A regra vem do `PersonValidator` e vale
para POST e PUT.

---

## 📋 Melhorias Futuras

- **Frontend React** — interface para consumir a API (o CORS já está preparado)
- **Docker** — containerizar a aplicação junto com o SQL Server
- **Testes de integração HTTP** — `WebApplicationFactory` para cobrir o pipeline completo
- **Serilog** — log em arquivo com rotação, sobre as chamadas `ILogger` já existentes
