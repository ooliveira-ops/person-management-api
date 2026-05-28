using System;
using System.ComponentModel.DataAnnotations;


namespace Api.Validators
{
	public class PersonValidator : ValidationAttribute                                                  //resumo: Esta classe é um validador personalizado para validar a data de nascimento de uma pessoa. Ela herda da classe ValidationAttribute e implementa o método IsValid para verificar se a data de nascimento não está no futuro. Se a data for válida, retorna true; caso contrário, retorna false. O método FormatErrorMessage é sobrescrito para fornecer uma mensagem de erro personalizada quando a validação falha.
	{
		public override bool IsValid(object value)
		{
			if (value is DateTime dateOfBirth)
			{
				return dateOfBirth <= DateTime.Now;
			}
			return false;
		}

		public override string FormatErrorMessage(string name)
		{
			return "DateOfBirth cannot be in the future";
		}
	}
}