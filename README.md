# Person Management API 👥

**Status:** 🟢 In Development (Core features completed)  
**Last Updated:** May 28, 2026  
**Delivery Date:** June 2, 2026

---

## 📋 Project Overview

A RESTful Web API built with **ASP.NET Core 8** for managing people and their addresses. The API demonstrates professional software architecture patterns, clean code practices, and comprehensive database integration.

**Purpose:** Technical assessment to evaluate code organization, best practices, API design, and developer understanding of the implementation.

---

## 🛠️ Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 8.0 | Framework |
| **ASP.NET Core** | 8.0 | Web API |
| **Entity Framework Core** | 8.0 | ORM (Object-Relational Mapping) |
| **SQL Server** | 2019+ | Database |
| **Swagger/OpenAPI** | 6.6.2 | API Documentation & Testing |
| **C#** | 12 | Language |

---

## 🏗️ Project Structure

```
person-management-api/
├── src/
│   └── Api/
│       ├── Controllers/
│       │   └── PersonsController.cs          # HTTP endpoints for CRUD operations
│       ├── Data/
│       │   └── AppDbContext.cs               # Entity Framework DbContext configuration
│       ├── DTOs/
│       │   ├── CreatePersonRequest.cs        # Data Transfer Object for creating person
│       │   ├── UpdatePersonRequest.cs        # Data Transfer Object for updating person
│       │   ├── PersonResponse.cs             # Data Transfer Object for API response
│       │   ├── AddressResponseDto.cs         # Address response DTO
│       │   ├── CreateAddressDto.cs           # Address creation DTO
│       │   └── UpdateAddressDto.cs           # Address update DTO
│       ├── Models/
│       │   ├── Person.cs                     # Person entity model
│       │   └── PersonAddress.cs              # PersonAddress entity model
│       ├── Repositories/
│       │   ├── IPersonRepository.cs          # Repository interface (contract)
│       │   └── PersonRepository.cs           # Repository implementation (data access)
│       ├── Validators/
│       │   └── PersonValidator.cs            # Custom validation for DateOfBirth
│       ├── Migrations/
│       │   └── [Migration files]             # Database schema history
│       ├── Program.cs                        # Application startup & configuration
│       ├── Api.csproj                        # Project file with NuGet references
│       ├── appsettings.json                  # Application settings
│       └── persons.db                        # SQLite database (development)
├── .gitignore
└── README.md                                  # This file
```

---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** installed ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server** 2019+ or **SQL Server Express** ([Download](https://www.microsoft.com/pt-br/sql-server/sql-server-editions-express))
- **Visual Studio 2022** or **VS Code** with C# extension

### Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/filipeoliveira-ops/person-management-api.git
cd person-management-api
```

#### 2. Restore Dependencies

```bash
dotnet restore
```

#### 3. Configure Database Connection

Edit `src/Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PersonManagementApi;User Id=sa;Password=your_password;TrustServerCertificate=true;"
  }
}
```

**Replace:**
- `your_password` with your SQL Server `sa` password

#### 4. Create Database & Apply Migrations

```bash
cd src/Api
dotnet ef database update
```

This command:
- ✅ Creates the `PersonManagementApi` database
- ✅ Creates `Persons` table
- ✅ Creates `PersonAddresses` table
- ✅ Sets up relationships and constraints

#### 5. Run the Application

```bash
dotnet run
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5164
      https://localhost:7087
```

#### 6. Access Swagger UI

Open your browser: **http://localhost:5164/swagger**

---

## 📚 API Endpoints

### 1. Create a New Person
**Request:**
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
    "country": "Brazil"
  }
}
```

**Response:** `201 Created`
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
    "country": "Brazil"
  }
}
```

---

### 2. Get All Persons (with Pagination)
**Request:**
```
GET /api/Persons?page=1&pageSize=10&search=
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "João Silva",
    "dateOfBirth": "1990-05-15T00:00:00",
    "address": { /* address object */ }
  }
]
```

**Query Parameters:**
- `page` (optional, default: 1) - Page number for pagination
- `pageSize` (optional, default: 10) - Items per page
- `search` (optional) - Search by name, city, or state

---

### 3. Get Person by ID
**Request:**
```
GET /api/Persons/{id}
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "João Silva",
  "dateOfBirth": "1990-05-15T00:00:00",
  "address": { /* address object */ }
}
```

**Error Response:** `404 Not Found`
```json
{
  "message": "Person not found"
}
```

---

### 4. Update a Person
**Request:**
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
    "country": "Brazil"
  }
}
```

**Response:** `200 OK`

---

### 5. Delete a Person
**Request:**
```
DELETE /api/Persons/{id}
```

**Response:** `204 No Content` (success, no body)

**Error Response:** `404 Not Found`

---

## 🏛️ Architecture & Design Patterns

### Repository Pattern
The application uses the **Repository Pattern** to abstract data access logic:

```
Controller → IPersonRepository (interface) → PersonRepository (implementation) → DbContext → SQL Server
```

**Benefits:**
- ✅ Separates business logic from data access logic
- ✅ Makes testing easier (can mock the repository)
- ✅ Easier to change database providers (SQLite → SQL Server)
- ✅ Centralized data access methods

### Data Transfer Objects (DTOs)
DTOs are used for API requests/responses:
- `CreatePersonRequest` - Request body for POST
- `UpdatePersonRequest` - Request body for PUT
- `PersonResponse` - Response body for GET
- `AddressResponseDto` - Nested address in response

**Benefits:**
- ✅ Decouples API contracts from database models
- ✅ Validation happens at API layer
- ✅ Security (never expose all entity properties)

### Entity Models
- `Person` - Core entity with Id, Name, DateOfBirth, AddressId
- `PersonAddress` - Address entity with street, number, city, state, country
- **Relationship:** One-to-One (Person has one Address)

### Dependency Injection
All services are registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
```

When a controller needs `IPersonRepository`, the framework automatically provides `PersonRepository` instance.

---

## ✅ Validations Implemented

### 1. DateOfBirth Validation
```csharp
if (request.DateOfBirth > DateTime.Now)
    return BadRequest(new { message = "DateOfBirth cannot be in the future" });
```

### 2. Data Annotations
```csharp
[Required]
[MinLength(3)]
public string Name { get; set; }
```

### 3. Custom Validator
```csharp
[PersonValidator]
public DateTime DateOfBirth { get; set; }
```

---

## 📊 Database Schema

### Persons Table
| Column | Type | Constraints |
|--------|------|-------------|
| Id | INT | Primary Key, Auto-increment |
| Name | NVARCHAR(MAX) | NOT NULL |
| DateOfBirth | DATETIME | NOT NULL |
| AddressId | INT | Foreign Key |

### PersonAddresses Table
| Column | Type | Constraints |
|--------|------|-------------|
| Id | INT | Primary Key, Auto-increment |
| Street | NVARCHAR(MAX) | NOT NULL |
| Number | NVARCHAR(MAX) | NOT NULL |
| Complement | NVARCHAR(MAX) | Nullable |
| City | NVARCHAR(MAX) | NOT NULL |
| State | NVARCHAR(MAX) | NOT NULL |
| Country | NVARCHAR(MAX) | NOT NULL |

**Relationships:**
- Person → PersonAddress: One-to-One
- ON DELETE: Cascade (deleting a Person also deletes its Address)

---

## 📈 Git Commits History

All commits follow the format: `type: description`

```
✅ chore: initialize Web API project with folder structure
✅ chore: create project folder structure
✅ feat: create Person and PersonAddress models
✅ feat: create AppDbContext with EF Core configuration
✅ feat: create initial database migration and apply to SQLite
✅ feat: implement repository pattern with PersonRepository
✅ feat: add data annotations validation to DTOs
✅ feat: implement controllers, DTOs, and validators with fixes
✅ feat: test all CRUD endpoints on Swagger - all working correctly
✅ feat: configure SQL Server connection and apply migrations
✅ fix: resolve database schema issues and fix autoincrement configuration
```

---

## ✨ What's Completed

### Phase 1: Project Setup ✅
- [x] Project initialization with proper folder structure
- [x] Models (Person, PersonAddress) with relationships
- [x] Entity Framework Core DbContext configuration

### Phase 2: Data Access Layer ✅
- [x] Repository Pattern implementation (IPersonRepository, PersonRepository)
- [x] Database migrations with SQL Server
- [x] CRUD operations (Create, Read, Update, Delete)
- [x] Pagination support in GET All endpoint
- [x] Search functionality (by name, city, state)

### Phase 3: API Layer ✅
- [x] 5 REST endpoints implemented
- [x] DTOs for request/response
- [x] Input validation (Required, MinLength, Custom)
- [x] Error handling (404, 400, 500)
- [x] HTTP status codes (201, 200, 204, 400, 404)
- [x] Swagger integration for API documentation

### Phase 4: Testing & Verification ✅
- [x] All endpoints tested in Swagger
- [x] Database connectivity verified
- [x] Data persistence confirmed

---

## 📋 Remaining Work (TODO)

### High Priority
- [ ] **Unit Tests** - Xunit tests for Repository and Controller
  - EstimatedTime: 2-3 hours
  - Tests needed: GetByIdAsync, GetAllAsync, CreateAsync, UpdateAsync, DeleteAsync

### Medium Priority
- [ ] **FluentValidation** - Replace Data Annotations with FluentValidation
  - EstimatedTime: 1-2 hours
  - Benefits: More powerful validation rules, centralized validation

- [ ] **Standardized Response** - Implement ApiResponse wrapper
  - EstimatedTime: 1 hour
  - Includes: Success/error status, data, error messages

### Nice to Have
- [ ] **Pagination DTO** - Dedicated pagination response object
- [ ] **Global Exception Handler** - Middleware for exception handling
- [ ] **Logging** - Serilog integration
- [ ] **CORS** - Cross-Origin Resource Sharing configuration
- [ ] **API Versioning** - Support for multiple API versions

---

## 🧪 Testing the API

### Using Swagger UI (Recommended)
1. Run the application: `dotnet run`
2. Open browser: http://localhost:5164/swagger
3. Click on any endpoint
4. Click "Try it out"
5. Fill in the request body
6. Click "Execute"

### Using cURL
```bash
# Create a person
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
      "country": "Brazil"
    }
  }'

# Get all persons
curl http://localhost:5164/api/Persons

# Get person by ID
curl http://localhost:5164/api/Persons/1

# Update a person
curl -X PUT http://localhost:5164/api/Persons/1 \
  -H "Content-Type: application/json" \
  -d '{"name": "Maria Santos", ...}'

# Delete a person
curl -X DELETE http://localhost:5164/api/Persons/1
```

---

## 🐛 Troubleshooting

### Issue: "Cannot connect to database"
**Solution:** 
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Confirm database credentials (User Id, Password)
- Run `dotnet ef database update` to create database

### Issue: "DateOfBirth cannot be in the future"
**Solution:**
- Use a past date for DateOfBirth
- Format: YYYY-MM-DD

### Issue: "Person not found (404)"
**Solution:**
- Verify the ID exists in database
- Check SQL Server Management Studio

---

## 📚 Key Concepts Learned

1. **Repository Pattern** - Data access abstraction
2. **Dependency Injection** - ASP.NET Core DI container
3. **Entity Framework Core** - ORM for database operations
4. **DTOs** - Decoupling API contracts from entities
5. **Async/Await** - Non-blocking database operations
6. **RESTful API Design** - HTTP methods, status codes, resource naming
7. **Validations** - Data annotations and custom validators
8. **Database Migrations** - Version control for schema changes
9. **Swagger/OpenAPI** - API documentation and testing

---

## 📞 Contact & Support

**Intern:** Filipe Oliveira  
**Project Date:** May 21 - June 2, 2026  
**Supervisor:** [Supervisor Name]

---

## 📄 License

This project is for educational and assessment purposes.

---

**Last Updated:** May 28, 2026 - 02:50 AM  
**Status:** 🟢 Core Features Complete | 🟡 Testing & Refinement Phase
