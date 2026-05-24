using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
	public class UpdatePersonRequest										//(5) DTO de requisição para atualizar os dados de uma pessoa, incluindo seu endereço. Ele é semelhante ao CreatePersonRequest, mas pode ser usado para atualizar os dados existentes.
	{

		[Required(ErrorMessage = "Name is required")]
		[MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "DateOfBirth is required")]
		public DateTime DateOfBirth { get; set; }

		[Required(ErrorMessage = "Address is required")]																	//Campo Address é obrigatório para a atualização, pois os dados de endereço também precisam ser fornecidos para atualizar a pessoa.
		public UpdateAddressDto Address { get; set; }
	}


	public class UpdateAddressDto                                                   //DTO de solicitação para atualizar um endereço, utilizado dentro do UpdatePersonRequest para representar os dados de endereço associados à pessoa que está sendo atualizada.
	{

		[Required(ErrorMessage = "Street is required")]
		public string Street{  get; set; }

		[Required(ErrorMessage = "Number is required")]
		public string Number { get; set; }

		//"opcional"
		public string Complement { get; set; }

		[Required(ErrorMessage = "City is required")]
		public string City { get; set; }	

		[Required(ErrorMessage = "State is required")]
		public string State { get; set; }

		[Required(ErrorMessage = "Country is required")]
		public string Country { get; set; }	
	}
}
