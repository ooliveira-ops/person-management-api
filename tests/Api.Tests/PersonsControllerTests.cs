using System;
using Api.Controllers;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Api.Response;
using Api.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Api.Tests
{
	// Testes do PersonsController. Aqui o repositório é MOCKADO de propósito: o objeto
	// sob teste é o controller, e o repositório é dependência externa. O PersonValidator
	// vai real por ser uma classe pura, sem dependências.
	public class PersonsControllerTests
	{
		[Fact]
		public async Task GetPersonById_ShouldReturnAddressAsNull_WhenPersonHasNoAddress()
		{
			// arrange - pessoa SEM endereço: o estado que derrubava o MapToResponse.
			// Só o mock consegue produzi-lo, já que a FK PersonId é obrigatória no banco.
			var mockRepository = new Mock<IPersonRepository>();
			var person = new Person
			{
				Id = 1,
				Name = "Joeder Barreiro",
				DateOfBirth = new DateTime(1990, 5, 15),
				Address = null							 // Simulando pessoa sem endereço
			};
			// Configurando o mock para retornar a pessoa sem endereço quando GetByIdAsync for chamado com o ID 1
			mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(person);

			// mock do repositório + validador real. O NullLogger descarta os logs: este
			// teste não é sobre logging, e ILogger expõe LogWarning como método de
			// extensão, o que impede verificá-lo com Mock.Verify.
			var controller = new PersonsController(
				mockRepository.Object,
				new PersonValidator(),
				NullLogger<PersonsController>.Instance);

			// act
			var result = await controller.GetPersonById(1);

			// assert - 200 com address nulo, em vez de NullReferenceException
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var response = okResult.Value.Should().BeOfType<ApiResponse<PersonResponse>>().Subject;

			response.Success.Should().BeTrue();
			response.Data!.Name.Should().Be(person.Name);
			response.Data.Address.Should().BeNull();
		}



		[Fact]
		public async Task UpdatePerson_ShouldReturnBadRequest_WhenDateOfBirthIsInTheFuture()
		{
			// arrange - a pessoa existe, então o 400 só pode vir da validação do PersonValidator
			var mockRepository = new Mock<IPersonRepository>();
			var person = new Person
			{
				Id = 1,
				Name = "Vitor Santos",
				DateOfBirth = new DateTime(2011, 1, 1)
			};
			mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(person);

			// mock do repositório + validador real. O NullLogger descarta os logs: este
			// teste não é sobre logging, e ILogger expõe LogWarning como método de
			// extensão, o que impede verificá-lo com Mock.Verify.
			var controller = new PersonsController(
				mockRepository.Object,
				new PersonValidator(),
				NullLogger<PersonsController>.Instance);

			var request = new UpdatePersonRequest
			{
				Name = "Vitor Santos",
				DateOfBirth = DateTime.Now.AddYears(1), // data no futuro; AddYears evita uma data fixa que envelhece
				Address = null
			};

			// act
			var result = await controller.UpdatePerson(1, request);

			// assert - 400 com a mensagem do PersonValidator
			var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
			var response = badRequest.Value.Should().BeOfType<ApiResponse<PersonResponse>>().Subject;

			response.Success.Should().BeFalse();
			response.Message.Should().Contain("DateOfBirth cannot be in the future");

			// e o mais importante: rejeitou ANTES de gravar
			mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Person>()), Times.Never); 
		}
	}
}
