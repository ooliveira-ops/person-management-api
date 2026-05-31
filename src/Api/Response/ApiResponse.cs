using System;


namespace Api.Response
{
	public class ApiResponse<T>
	{
		public bool Success { get; set; }
		public string? Message { get; set; }													//"?" = 'pode ser nulo'
		public T? Data { get; set; }                                             //"T" que vai receber os dados de resposta, seja uma pessoa, uma lista de pessoas ou qualquer outro tipo de dado que a API possa retornar. Ele é definido como um tipo genérico para permitir flexibilidade na estrutura da resposta, permitindo que diferentes tipos de dados sejam retornados dependendo do contexto da solicitação.


																													//"static" Pode ser chamado sem instanciar a classe. Ex resumido: ApiResponse<string>.SuccessResponse("Data loaded successfully"); )
		public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")               //"Método para retorno uma resposta de sucesso
		{
			return new ApiResponse<T>
			{
				Success = true,
				Message = message,
				Data = data
			};
		}


		public static ApiResponse<T> ErrorResponse(string message)                                             //"Método para retorno uma resposta de erro
		{
			return new ApiResponse<T>
			{
				Success = false,
				Message = message,
				Data = default(T)                                                                               //"default(T)" resumo: Retorna o valor padrão para o tipo T. Se T for um tipo de referência, isso será null. Se T for um tipo de valor, como int ou bool, isso retornará 0 ou false, respectivamente. Isso é útil para indicar que não há dados válidos a serem retornados em caso de erro.
			};
		}
	}
}