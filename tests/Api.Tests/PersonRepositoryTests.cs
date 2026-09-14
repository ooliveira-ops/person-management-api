using System;
using Api.Data;
using Api.Models;
using Api.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests
{
	public class PersonRepositoryTests 
	{
		// Cria um banco SQLite novo em memória para cada teste, garantindo isolamento.
		private static AppDbContext NewContext()
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			// o banco só existe enquanto houver conexão aberta; fechá-la o destrói
			connection.Open();
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlite(connection)
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
		public async Task UpdateAsync_ShouldUpdatePerson_WhenValidDataProvided()
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

		[Fact]
		public async Task SearchAsync_ShouldReturnFilteredPersons_WhenSearchTermProvided()
		{
			// "arrange" - Carlos Pereira é o controle negativo: sem alguém que o filtro
			// precise EXCLUIR, o teste passaria mesmo que o Where fosse removido
			using var context = NewContext();
			var repository = new PersonRepository(context);
			var persons = new List<Person>
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
			// AddRange marca as entidades; quem grava no banco é o SaveChanges - nesta ordem
			context.Persons.AddRange(persons);
			await context.SaveChangesAsync();

			//"act" - termo "Silva", 1ª página, 10 itens por página
			var result = await repository.SearchAsync("Silva", 1, 10);

			// "assert" - HaveCount garante que nada sobrou; OnlyContain, que nada indevido entrou
			result.Should().HaveCount(2);
			result.Should().OnlyContain(p => p.Name.Contains("Silva"));
		}




		[Fact]
		public async Task GetAllAsync_ShouldLimitPageSize_WhenPageSizeExceedsMaximum()
		{
			// "arrange" - o teto é 100, então preciso de MAIS de 100 registros:
			// com 10 pessoas o teste passaria com ou sem o limite
			using var context = NewContext();
			var repository = new PersonRepository(context);

			var pessoas = Enumerable.Range(1, 105)
				.Select(i => NovaPessoa($"Pessoa {i}"))
				.ToList();

			context.Persons.AddRange(pessoas);
			await context.SaveChangesAsync();

			// "act" - pede muito acima do teto
			var result = await repository.GetAllAsync(1, 99999);

			// "assert" - o Math.Clamp corta em 100, mesmo havendo 105 no banco
			result.Should().HaveCount(100);
		}



		// Evita repetir o bloco de endereço em cada teste
		private static Person NovaPessoa(string nome)
		{
			return new Person
			{
				Name = nome,
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = new PersonAddress
				{
					Street = "Rua A",
					Number = "123",
					Complement = "",
					City = "São Paulo",
					State = "SP",
					Country = "Br"
				}
			};
		}
	}	
}