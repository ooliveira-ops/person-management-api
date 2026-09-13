using System;
using Api.DTOs;

namespace Api.DTOs
{
	// Formato de saída da API, produzido por PersonsController.MapToResponse.
	// Diferente da entidade Person, não tem navegação de volta para o endereço-pai,
	// o que evita referência circular na serialização JSON.
	public class PersonResponse
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public DateTime DateOfBirth { get; set; }

		// Anulável: pessoas sem endereço cadastrado saem com "address": null.
		public AddressResponseDto? Address { get; set; }
	}

	// Endereço aninhado na resposta.
	public class AddressResponseDto
	{
		public int Id { get; set; }
		public string? Street { get; set; }
		public string? Number { get; set; }
		public string? Complement { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? Country { get; set; }
	}
}
