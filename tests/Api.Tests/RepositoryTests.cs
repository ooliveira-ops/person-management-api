using System;
using Xunit;
using Moq;
using FluentAssertions;
using Api.Repositories;
using Api.Models;

namespace Api.Tests
{
	public class PersonRepositoryTests
	{
		[Fact]
		public async Task GetByIdAsync_ShouldReturnPerson_WhenPersonExists()                //"GetByIdAsync"(nome do método testado) - "ShouldReturnPerson"(Deve retornar pessoa) - "WhenPersonExists"(Quando a pessoa existe)
		{
			// "arrange" (prepração)
			var mockRepository = new Mock<IPersonRepository>();
			var person = new Person
			{
				Id = 1,
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
			};

			mockRepository.Setup(repo => repo.GetByIdAsync(1))                                  // Configura o comportamento do método GetByIdAsync para retornar a pessoa quando o ID for 1
			.ReturnsAsync(person);                                                              // Chama o método GetByIdAsync do repositório simulado para obter a pessoa com ID 1

			// "act"	(ação)																		// Chama o método GetByIdAsync do repositório simulado para obter a pessoa com ID 1
			var result = await mockRepository.Object.GetByIdAsync(1);

			// "assert"	(verificação)																		// resumo: Verifica se o resultado não é nulo, se o nome da pessoa é "João Silva" e se o ID é 1, garantindo que o método GetByIdAsync retorna os dados corretos para a pessoa solicitada.
			result.Should().NotBeNull();
			result.Name.Should().Be("João Silva");
			result.Id.Should().Be(1);
		}



		[Fact]
		public async Task CreateAsync_ShouldCreatePerson_WhenValidProvided()                        //"CreateAsync"(nome do método testado) - "ShouldCreatePerson"(Deve criar pessoa) - "WhenValidProvided"(Quando os dados válidos são fornecidos)
		{
			//"arrange" 
			var mockRepository = new Mock<IPersonRepository>();                                     //cria o fake do repositório
			var person = new Person
			{
				Id = 1,
				Name = "Maria Santos",
				DateOfBirth = new DateTime(1995, 3, 20)
			};

			// "act"
			await mockRepository.Object.CreateAsync(person);

			//assert
			mockRepository.Verify(r => r.CreateAsync(It.IsAny<Person>()), Times.Once);                  //"CreateAsync"(nome do método testado) - "It.IsAny<Person>()"(Verifica se o método CreateAsync foi chamado com qualquer objeto do tipo Person) - "Times.Once"(Verifica se o método foi chamado exatamente uma vez)
		}



		[Fact]
		public async Task UpdateAsync_ShouldUptadePerson_WhenValidDataProvied()
		{

			//"arrange"
			var mockRepository = new Mock<IPersonRepository>();
			var updatedPerson = new Person
			{ 
				Id = 1,
				Name = "João Silva Atualizado",
				DateOfBirth = new DateTime(1990, 5, 15)
			};

			//"act"
			await mockRepository.Object.UpdateAsync(updatedPerson);

			//"assert"
			mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Person>()), Times.Once);                 
		}
	}	
}