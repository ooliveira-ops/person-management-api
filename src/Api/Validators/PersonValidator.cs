using System;
using Api.Models;
using FluentValidation;

namespace Api.Validators
{
	public class PersonValidator : AbstractValidator<Person>                                                //resumo: "PersonValidator" é uma classe de validação que herda de AbstractValidator<Person>, fornecida pela biblioteca FluentValidation. Ela define as regras de validação para a entidade Person, garantindo que os dados sejam válidos antes de serem processados ou armazenados no banco de dados.
	{
		public PersonValidator()                                                                            //Construtor da classe PersonValidator, onde as regras de validação são definidas para as propriedades da entidade Person.
		{
			RuleFor(x => x.Name)                                                                            // Define a regra de validação para a propriedade "Name" da entidade Person. Ela especifica que o nome não pode ser vazio e deve ter pelo menos 3 caracteres. Se a validação falhar, a mensagem "Name must be at least 3 characters" será retornada.
				.NotEmpty()
				.MinimumLength(3)
				.WithMessage("Name must be at least 3 characters");

			RuleFor(x => x.DateOfBirth)                                                                     //"x => x.DateOfBirth" é uma expressão lambda que representa a propriedade "DateOfBirth" da entidade Person. Ela define a regra de validação para a data de nascimento, especificando que ela deve ser menor ou igual à data atual (DateTime.Now). Se a validação falhar, a mensagem "DateOfBirth cannot be in the future" será retornada.
				.LessThanOrEqualTo(DateTime.Now)
				.WithMessage("DateOfBirth cannot be in the future");
		}
	}
}