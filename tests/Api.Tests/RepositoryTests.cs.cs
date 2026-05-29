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
		public async Task GetByIdAsync_ShouldReturnPerson_WhenPersonExists()
		{
			// "arrange"
			var mockRepository = new Mock<IPersonRepository>();
			var person = new Person
			{
				Id = 1,
				Name = "João Silva",
				DateOfBirth = new DateTime(1990, 5, 15),
			};

			mockRepository.Setup(repo => repo.GetByIdAsync(1))                                  // Configura o comportamento do método GetByIdAsync para retornar a pessoa quando o ID for 1
			.ReturnsAsync(person);                                                              // Chama o método GetByIdAsync do repositório simulado para obter a pessoa com ID 1

			// "act"																			// Chama o método GetByIdAsync do repositório simulado para obter a pessoa com ID 1
			var result = await mockRepository.Object.GetByIdAsync(1);

			// "assert"																			// resumo: Verifica se o resultado não é nulo, se o nome da pessoa é "João Silva" e se o ID é 1, garantindo que o método GetByIdAsync retorna os dados corretos para a pessoa solicitada.
			result.Should().NotBeNull();
			result.Name.Should().Be("João Silva");
			result.Id.Should().Be(1);
		}
	}
}