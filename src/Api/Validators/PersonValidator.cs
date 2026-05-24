using System;
using System.ComponentModel.DataAnnotations;


namespace Api.Validators
{
	public class PersonValidator																		//sobre o método de validação da data de nascimento
	{
		public static ValidationResult ValidateDateOfBirth(DateTime dateOfBirth)
		{
			if (dateOfBirth > DateTime.Now)
			{
				return new ValidationResult("DateOfBirth cannot be in the future");
			}
			return ValidationResult.Success;
		}
	}
}