using System;

namespace Api.Response
{
	// Envelope padrão de toda resposta da API. Garante que sucesso e erro tenham o mesmo
	// formato, para o cliente checar sempre "success" em vez de adivinhar pelo status code.
	// <T> é o tipo do payload: uma pessoa, uma lista de pessoas, o que o endpoint devolver.
	public class ApiResponse<T>
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public T? Data { get; set; }

		// Fábricas estáticas: chamadas sem instanciar a classe, como
		// ApiResponse<PersonResponse>.SuccessResponse(pessoa).
		public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
		{
			return new ApiResponse<T>
			{
				Success = true,
				Message = message,
				Data = data
			};
		}

		public static ApiResponse<T> ErrorResponse(string message)
		{
			return new ApiResponse<T>
			{
				Success = false,
				Message = message,
				// default(T) é null para tipos de referência e 0/false para tipos de valor.
				Data = default(T)
			};
		}
	}
}
