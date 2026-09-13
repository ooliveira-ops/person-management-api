using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
	// Corpo esperado no PUT /api/Persons/{id}.
	// Espelha o CreatePersonRequest: o PUT substitui o recurso inteiro, então todos os
	// campos obrigatórios precisam vir de novo, não só os que mudaram.
	public class UpdatePersonRequest
	{
		[Required(ErrorMessage = "Name is required")]
		[MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
		public string? Name { get; set; }

		[Required(ErrorMessage = "DateOfBirth is required")]
		public DateTime DateOfBirth { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public UpdateAddressDto? Address { get; set; }
	}

	// Endereço aninhado no corpo do PUT.
	public class UpdateAddressDto
	{
		[Required(ErrorMessage = "Street is required")]
		public string? Street{  get; set; }

		[Required(ErrorMessage = "Number is required")]
		public string? Number { get; set; }

		// Opcional, como no CreateAddressDto.
		public string? Complement { get; set; }

		[Required(ErrorMessage = "City is required")]
		public string? City { get; set; }

		[Required(ErrorMessage = "State is required")]
		public string? State { get; set; }

		[Required(ErrorMessage = "Country is required")]
		public string? Country { get; set; }
	}
}
