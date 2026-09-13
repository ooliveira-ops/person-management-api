using System;
using Api.Models;
using FluentValidation;

namespace Api.Validators
{
	// Regras de validação da entidade Person, escritas com FluentValidation.
	//
	// Fonte única das regras de negócio: o PersonsController injeta este validador e o
	// aplica tanto no POST quanto no PUT. Antes, a checagem de data futura era feita à
	// mão apenas no POST, e o PUT aceitava data de nascimento no futuro.
	// Ao mudar uma regra aqui, os dois endpoints mudam juntos.
	public class PersonValidator : AbstractValidator<Person>
	{
		public PersonValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MinimumLength(3)
				.WithMessage("Name must be at least 3 characters");

			RuleFor(x => x.DateOfBirth)
				.LessThanOrEqualTo(DateTime.Now)
				.WithMessage("DateOfBirth cannot be in the future");
		}
	}
}
