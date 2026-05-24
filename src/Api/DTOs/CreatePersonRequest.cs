using System;
using Api.DTOs;
using Api.Validators;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
	public class CreatePersonRequest													//(4) DTO de solicitação para criar uma pessoa
	{

		[Required(ErrorMessage = "Name is required")]										//"nome é necessário"
		[MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
		public string? Name { get; set; }                                                   //"?" ex: field(campo) Name: ele pode conter um valor de string ou pode ser null.

		[Required(ErrorMessage = "DateOfBirth is required")]
		[CustomValidation(typeof(PersonValidator), nameof(PersonValidator.ValidateDateOfBirth),                 //Garante que a data de nascimento seja uma data válida e não seja uma data futura. (pega a classe PersonValidator e o método ValidateDateOfBirth para validar a data de nascimento fornecida)
		ErrorMessage = "DateOfBirth cannot be in the future")]		
		public DateTime DateOfBirth { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public CreateAddressDto? Address { get; set; }
	}

	public class CreateAddressDto                                                       //DTO de solicitação para criar um endereço, utilizado dentro do CreatePersonRequest para representar os dados de endereço associados à pessoa que está sendo criada.
	{
		
		[Required(ErrorMessage = "Street is required")]		
		public string? Street { get; set; }                                             

		[Required(ErrorMessage = "Number is required")]
		public string? Number { get; set; }

		//este era opcnional, então não tem o "[Required]"
		public string? Complement { get; set; }
		
		[Required(ErrorMessage = "City is required")]
		public string? City { get; set; }

		[Required(ErrorMessage = "State is required")]
		public string? State { get; set; }

		[Required(ErrorMessage = "Country is required")]
		public string? Country { get; set; }
	}
}