using System;
using System.Net;
using Api.Data;
using Api.Models;
using Api.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Api.Tests
{
	public class PersonRepositoryTests 
	{
		//resumo: Método auxiliar para criar um novo contexto de banco de dados em memória para os testes.
		private static AppDbContext NewContext()
		{
			var conection = new SqliteConnection("DataSource=:memory:");    //cria uma conexão com um banco de dados SQLite em memória
			conection.Open();   //mantem o banco de dados em memória aberto durante o teste
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlite(conection)
				.Options;
			var context = new AppDbContext(options);
			context.Database.EnsureCreated();  //garante que o banco de dados em memória seja criado antes do teste
			return context;
		}
		


		//"GetByIdAsync"(nome do método testado) - "ShouldReturnPerson"(Deve retornar pessoa) -
		//"WhenPersonExists"(Quando a pessoa existe)
		[Fact]
		public async Task GetByIdAsync_ShouldReturnPerson_WhenPersonExists()										 
		{
			// "arrange" (preparação) - Com o repositório real, não simulado, usando o contexto em memória para criar uma pessoa no banco de dados em memória
			using var context = NewContext();
			var repository = new PersonRepository(context); //cria uma instância do repositório de pessoas usando o contexto em memória
			var person = new Person
			{
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = new PersonAddress
				{
					Street = "Rua A",
					Number = "123",
					Complement = "",
					City = "Sp",
					State = "SP",
					Country = "Br"
				}
			};
			// grava a pessoa (e o endereço junto) no banco em memória através do repositório real
			await repository.CreateAsync(person);

			// "act"	(ação)		"await = aguarda/espera a resposta"
			// chama o método GetByIdAsync do repositório para buscar a pessoa com o ID gerado pelo banco
			var result = await repository.GetByIdAsync(person.Id);

			// "assert"	(verificação)																				
			result.Should().NotBeNull();			
			result.Name.Should().Be("João Silva");
			result.Address.Should().NotBeNull();	// prova que o Include funciona
			// resumo: Verifica se o resultado não é nulo, se o nome da pessoa é "João Silva"
			// garantindo que o método GetByIdAsync retorna os dados corretos para a pessoa solicitada.
		}


		//"CreateAsync"(nome do método testado) - "ShouldCreatePerson"(Deve criar pessoa) -
		//"WhenValidProvided"(Quando os dados válidos são fornecidos)
		[Fact]
		public async Task CreateAsync_ShouldCreatePerson_WhenValidProvided()											
		{
			//"arrange" 
			// cria um objeto Person com os dados da pessoa a ser criada
			using var context = NewContext();
			var repository = new PersonRepository(context);
			var person = new Person
			{
				Name = "Maria Santos",
				DateOfBirth = new DateTime(1995, 3, 20),
				Address = new PersonAddress
				{
					Street = "Rua x",
					Number = "456",
					Complement = "",
					City = "RIo",
					State = "RJ",
					Country = "Br"
				}
			};

			// "act"
			// chama o método CreateAsync do repositório real para criar a pessoa com os dados fornecidos
			await repository.CreateAsync(person);

			//assert - consulta o Banco de dados em memória para verificar se a pessoa foi criada corretamente
			var saved = await context.Persons.AsNoTracking()    //"Sem rastreamento" = a consulta vai ao banco de dados
				.FirstOrDefaultAsync(p => p.Name == "Maria Santos");
			saved.Should().NotBeNull();
			saved.Id.Should().BeGreaterThan(0);
			// "Seja maior que":
			//verifica se o ID da pessoa criada é maior que 0, indicando que foi salva corretamente
		}



		[Fact]
		public async Task UpdateAsync_ShouldUptadePerson_WhenValidDataProvied()
		{

			//"arrange" - primeiro precisa existir uma pessoa(criar antes para atualizar)
			using var context = NewContext();
			var repository = new PersonRepository(context);
			var person = new Person
			{ 
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = new PersonAddress
				{
					Street = "Rua A",
					Number = "123",
					Complement = "",
					City = "Sp",
					State = "SP",
					Country = "Br"
				}
			};
			await repository.CreateAsync(person);

			//"act"
			// altera o nome DEPOIS de salvar - sem esta linha nada muda e o assert falha
			person.Name = "João Silva Atualizado";
			// chama o método UpdateAsync do repositório real para gravar a alteração no banco
			await repository.UpdateAsync(person);

			//"assert"
			// consulta o Banco de dados em memória para verificar se a pessoa foi atualizada corretamente
			var updated = await context.Persons.AsNoTracking()          
				.FirstOrDefaultAsync(p => p.Id == person.Id);
			updated.Name.Should().Be("João Silva Atualizado");
		}


		[Fact]
		public async Task DeleteAsync_ShouldDeletePerson_WhenValidIdProvided()
		{
			// cria o contexto e o repositório reais sobre o banco em memória
			using var context = NewContext();
			var repository = new PersonRepository(context);
			var person = new Person
			{
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = new PersonAddress
				{
					Street = "Rua A",
					Number = "123",
					Complement = "",
					City = "Sp",
					State = "SP",
					Country = "Br"
				}
			};
			await repository.CreateAsync(person);

			//act
			await repository.DeleteAsync(person.Id);

			// assert
			(await context.Persons.CountAsync()).Should().Be(0);  //verifica se não há mais pessoas no banco (Pois a cada teste, o banco é recriado do zero)
			(await context.PersonAddresses.CountAsync()).Should().Be(0);  //verifica se não há mais endereços após a exclusão
		}

		// "SearchAsync"(nome do método testado) - "ShouldReturnFilteredPersons"(Deve retornar pessoas filtradas) -
		//"WhenSearchTermProvided"(Quando um termo de pesquisa é fornecido)


		[Fact]
		public async Task SearchAsync_ShouldReturnFilteredPersons_WhenSearchTermProvided()
		{
			// "arrange"
			using var context = NewContext();
			var repository = new PersonRepository(context);
			var person = new List<Person>
			{
				new Person
				{
					Name = "João Silva",
					DateOfBirth = new DateTime(1990, 5, 15),
					Address = new PersonAddress
					{
						Street = "Rua A", Number = "123", Complement = "",
						City = "São Paulo", State = "SP", Country = "Br"
					}
				},
				new Person
				{
					Name = "Maria Silva",
					DateOfBirth = new DateTime(1985, 3, 10),
					Address = new PersonAddress
					{
						Street = "Rua B", Number = "456", Complement = "",
						City = "Rio de Janeiro", State = "RJ", Country = "Br"
					}
				},
				new Person
				{
					Name = "Carlos Pereira",
					DateOfBirth = new DateTime(1978, 11, 2),
					Address = new PersonAddress
					{
						Street = "Rua C", Number = "789", Complement = "",
						City = "Curitiba", State = "PR", Country = "Br"
					}
				}
			};
			context.Persons.AddRange(person); //adiciona as pessoas ao contexto
			await context.SaveChangesAsync();

			//"act"
			//vai chamar com o termo "Silva" e a paginação (1ª página, 10 itens por página)
			var result = await repository.SearchAsync("Silva", 1, 10);

			// "assert"
			result.Should().HaveCount(2);  //verifica se o resultado contém exatamente 2 pessoas)
			result.Should().OnlyContain(person => person.Name.Contains("Silva"));
		}
	}	
}