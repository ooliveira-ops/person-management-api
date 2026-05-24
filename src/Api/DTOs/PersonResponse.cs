using System;
using Api.DTOs;

namespace Api.DTOs
{
	public class PersonResponse												//(3) DTO de resposta para a entidade Person, que inclui os dados da pessoa e seu endereço
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public AddressResponseDto? Address { get; set; }
	}
	public class AddressResponseDto                                         //DTO de resposta para a entidade Address, utilizado dentro do PersonResponse para representar os dados de endereço associados à pessoa que está sendo retornada.
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
