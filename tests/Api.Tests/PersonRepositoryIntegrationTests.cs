using System;
using Api.Data;
using Api.Models;
using Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Api.Tests
{
	// Testes de INTEGRAÇÃO: rodam contra um SQL Server real, não contra o SQLite
	// in-memory dos testes unitários.
	public class PersonRepositoryIntegrationTests
	{
		private static readonly IConfiguration Configuration = new ConfigurationBuilder()
			.AddUserSecrets<PersonRepositoryIntegrationTests>(optional: true)
			.AddEnvironmentVariables()
			.Build();

		// Sem valor padrão de propósito: um default no código traria de volta a connection
		// string versionada. Como configurar está documentado no README.
		private static string TestConnectionString =>
			Configuration.GetConnectionString("TestDatabase")
			?? throw new InvalidOperationException(
				"Connection string 'TestDatabase' não configurada. Veja a seção de testes " +
				"de integração no README.");

		// Cria um contexto apontando para um banco de teste DEDICADO, recriado do zero a cada teste.
		private static AppDbContext NewSqlServerContext()
		{
			var connectionString = TestConnectionString;

			// O sufixo "_Tests" é essencial: a linha EnsureDeleted apaga o banco inteiro.
			// Como a connection string vem de fora, nada impediria alguém de apontar
			// para o banco de desenvolvimento.
			if (!connectionString.Contains("_Tests", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException(
					"A connection string 'TestDatabase' deve apontar para um banco com sufixo '_Tests'.");

			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			var context = new AppDbContext(options);
			context.Database.EnsureDeleted();   // apaga o banco de teste, se existir
			context.Database.Migrate();         // roda a pasta Migrations/
			return context;
		}



		[Fact]
		public async Task DeleteAsync_ShouldDeletePersonAndAddress_WhenPersonExists()
		{
			// arrange - banco recriado pelas migrations e uma pessoa com endereço
			using var context = NewSqlServerContext();
			var repository = new PersonRepository(context);
			var person = new Person
			{
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = new PersonAddress
				{
					Street = "Rua A",
					Number = "123",
					// prova que a migration MakeComplementNullable foi aplicada: sem ela,
					// a coluna seria NOT NULL e este INSERT falharia
					Complement = null,
					City = "São Paulo",
					State = "SP",
					Country = "Brasil"
				}
			};
			await repository.CreateAsync(person);

			// confirma que o arrange gravou de verdade, antes de testar a remoção
			(await context.Persons.CountAsync()).Should().Be(1);
			(await context.PersonAddresses.CountAsync()).Should().Be(1);


			// act - apaga a pessoa
			await repository.DeleteAsync(person.Id);


			// assert - as DUAS tabelas precisam ficar vazias
			(await context.Persons.CountAsync()).Should().Be(0);
			(await context.PersonAddresses.CountAsync()).Should().Be(0);
		}



		[Fact]
		public async Task GetAllAsync_ShouldReturnFirstPage_WhenPageNumberIsZero()
		{
			// arrange - uma pessoa basta: o que se testa aqui é o cálculo do Skip,
			// não o conteúdo da lista
			using var context = NewSqlServerContext();
			var repository = new PersonRepository(context);
			await repository.CreateAsync(new Person
			{
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = new PersonAddress
				{
					Street = "Rua XYZ",
					Number = "123",
					Complement = null,			
					City = "São Paulo",
					State = "SP",
					Country = "Brasil"
				}
			});

			// act - sem a normalização do repositório isto viraria Skip(-10), e o SQL Server
			// rejeita OFFSET negativo — era o erro 500 original da API
			var result = await repository.GetAllAsync(0, 10);

			// assert - page 0 é tratada como page 1, então a pessoa aparece
			result.Should().HaveCount(1);
		}
	}
}
