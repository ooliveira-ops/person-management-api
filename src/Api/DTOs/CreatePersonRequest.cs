using System;
using Api.DTOs;
using Api.Validators;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
	// Corpo esperado no POST /api/Persons.
	// DTO separado da entidade Person de propósito: o cliente não envia Id nem escolhe
	// chaves, e as regras de entrada podem mudar sem mexer no modelo do banco.
	// As Data Annotations abaixo são verificadas pelo [ApiController] antes do controller rodar.
	public class CreatePersonRequest
	{
		[Required(ErrorMessage = "Name is required")]
		[MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
		public string? Name { get; set; }

		[Required(ErrorMessage = "DateOfBirth is required")]
		public DateTime DateOfBirth { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public CreateAddressDto? Address { get; set; }
	}

	// Endereço aninhado no corpo do POST.
	public class CreateAddressDto
	{
		[Required(ErrorMessage = "Street is required")]
		public string? Street { get; set; }

		[Required(ErrorMessage = "Number is required")]
		public string? Number { get; set; }

		// Sem [Required]: o complemento é opcional. A coluna correspondente no banco
		// também aceita NULL — as duas pontas precisam concordar, ou um POST sem
		// complement falha na gravação.
		public string? Complement { get; set; }

		[Required(ErrorMessage = "City is required")]
		public string? City { get; set; }

		[Required(ErrorMessage = "State is required")]
		public string? State { get; set; }

		[Required(ErrorMessage = "Country is required")]
		public string? Country { get; set; }
	}
}
