using System;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Api.Response;

using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PersonsController : ControllerBase                                                                 //(6) Controlador para gerenciar as operações relacionadas à entidade Person. Ele utiliza os DTOs de solicitação e resposta para receber e enviar dados, e interage com o repositório para realizar as operações de CRUD.
	{
		private readonly IPersonRepository _repository;                                                              //O repositório é injetado através do construtor, permitindo que o controlador acesse os métodos de acesso a dados definidos na interface IPersonRepository.

		public PersonsController(IPersonRepository repository)
		{
			_repository = repository;                                                                                // O construtor recebe uma instância do repositório, que é fornecida pelo mecanismo de injeção de dependência do ASP.NET Core. Isso permite que o controlador utilize o repositório para realizar as operações necessárias.
		}



		[HttpPost]
		public async Task<ActionResult<ApiResponse<PersonResponse>>> CreatePerson(CreatePersonRequest request)                   //resumo: Método de ação para criar uma nova pessoa. Ele recebe um DTO de solicitação (CreatePersonRequest) contendo os dados necessários para criar uma pessoa, e retorna um DTO de resposta (PersonResponse) com os detalhes da pessoa criada.
		{
			if (request.DateOfBirth > DateTime.Now)
			{                                                                                                               //"BadRequest" = pedido mal formatado, ou seja, o cliente enviou dados que não fazem sentido (nascimento no futuro). O método retorna um status HTTP 400 Bad Request com uma mensagem de erro encapsulada em um objeto ApiResponse de erro.
				return BadRequest(ApiResponse<PersonResponse>.ErrorResponse("DateOfBirth cannot be in the future"));
			}

			var address = new PersonAddress                                                                             //resumo: Criação de um objeto PersonAddress a partir dos dados fornecidos no DTO de solicitação. O endereço é construído utilizando as propriedades do DTO, como Street, Number, Complement, City, State e Country. Esse objeto será associado à pessoa que está sendo criada.
			{
				Street = request.Address.Street,
				Number = request.Address.Number,
				Complement = request.Address.Complement,
				City = request.Address.City,
				State = request.Address.State,
				Country = request.Address.Country
			};


			var person = new Person                                                                              //resumo: Criação de um objeto Person utilizando os dados fornecidos no DTO de solicitação. O objeto Person é construído com as propriedades Name, DateOfBirth e o endereço criado anteriormente. Esse objeto representa a pessoa que será criada no sistema.
			{
				Name = request.Name,
				DateOfBirth = request.DateOfBirth,
				Address = address
			};
			await _repository.CreateAsync(person);                                                              //resumo: Chamada ao método CreateAsync do repositório para salvar a nova pessoa no banco de dados. O método é assíncrono, permitindo que a operação de criação seja realizada de forma eficiente sem bloquear o thread principal.

			var response = MapToResponse(person);                                                               //resumo: Mapeamento do objeto Person para um DTO de resposta (PersonResponse) utilizando o método MapToResponse. Esse método converte os dados da pessoa criada em um formato adequado para ser retornado ao cliente, incluindo os detalhes da pessoa e seu endereço.
			return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, ApiResponse<PersonResponse>.SuccessResponse(response, "Person created successfully"));    //Retorno do PersonResponse criado embrulhado no Response Padronizado (HTTP 201)
		}




		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<PersonResponse>>> GetPersonById(int id)                                  //resumo: Método de ação para obter uma pessoa por ID. Ele recebe um parâmetro de rota (id) e retorna um DTO de resposta (PersonResponse) contendo os detalhes da pessoa correspondente ao ID fornecido.
		{
			var person = await _repository.GetByIdAsync(id);                                                    //resumo: Chamada ao método GetByIdAsync do repositório para recuperar a pessoa do banco de dados com base no ID fornecido. O método é assíncrono, permitindo que a operação de recuperação seja realizada de forma eficiente sem bloquear o thread principal.
			if (person == null)
			{
				return NotFound(ApiResponse<PersonResponse>.ErrorResponse("Person not found"));                 //resumo: Verificação se a pessoa foi encontrada. Se o objeto person for nulo, significa que não existe uma pessoa com o ID fornecido, e o método retorna um status HTTP 404 Not Found com a mensagem de não encontrada encapsulada em um objeto ApiResponse de erro.
			}
			var response = MapToResponse(person);                                                                //resumo: Mapeamento do objeto Person para um DTO de resposta (PersonResponse) utilizando o método MapToResponse. Esse método converte os dados da pessoa recuperada em um formato adequado para ser retornado ao cliente, incluindo os detalhes da pessoa e seu endereço.
			return Ok(ApiResponse<PersonResponse>.SuccessResponse(response));                                     //Retorno do PersonResponse embrulhado no Response Padronizado (HTTP 200)
		}



		[HttpGet]
		public async Task<ActionResult<ApiResponse<List<PersonResponse>>>> GetPersons([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)     //resumo: Método de ação para obter uma lista de pessoas com suporte à paginação e filtragem por nome. Ele recebe parâmetros de consulta (page, pageSize e name) e retorna uma lista de DTOs de resposta (PersonResponse) contendo os detalhes das pessoas que correspondem aos critérios fornecidos.
		{
			List<Person> persons;                                                                                                   //Declara uma lista vazia (ainda sem dados)


			if (!string.IsNullOrEmpty(search))                                                                                  //"SE o search NÃO for vazio (ou seja, SE o usuário digitou algo pra buscar)"
			{
				persons = await _repository.SearchAsync(search, page, pageSize);												//"persons recebe apenas as pessoas que contém o termo buscado"
			}
			else                                                                                                                //"SENÃO (usuário não digitou nada pra buscar)"
			{
				persons = await _repository.GetAllAsync(page, pageSize);                                                           //"persons recebe TODAS as pessoas (com paginação)"
			}

			var response = persons.Select(MapToResponse).ToList();                                                               //Pega a lista persons (seja do if ou do else) e converte cada Person em PersonResponse usando o MapToResponse (que é um método do Controller mesmo, não do DTO!)
			return Ok(ApiResponse<List<PersonResponse>>.SuccessResponse(response));                                              //Retorno da lista de PersonResponse embrulhada no Response Padronizado (HTTP 200)
		}



		[HttpPut("{id}")]
		public async Task<ActionResult<ApiResponse<PersonResponse>>> UpdatePerson(int id, UpdatePersonRequest request)                   //resumo: Método de ação para atualizar os detalhes de uma pessoa existente. Ele recebe um parâmetro de rota (id) e um DTO de solicitação (UpdatePersonRequest) contendo os dados atualizados da pessoa, e retorna um DTO de resposta (PersonResponse) com os detalhes da pessoa atualizada.
		{
			var person = await _repository.GetByIdAsync(id);                                                    //resumo: Chamada ao método GetByIdAsync do repositório para recuperar a pessoa do banco de dados com base no ID fornecido. O método é assíncrono, permitindo que a operação de recuperação seja realizada de forma eficiente sem bloquear o thread principal.
			if (person == null)
			{
				return NotFound(ApiResponse<PersonResponse>.ErrorResponse("Person not found"));                                          //resumo: ApiResponse<PersonResponse> → Usando nosso wrapper -- .ErrorResponse("Person not found") → Cria resposta com: {success: false, message: "Person not found", data: null }
			}
			person.Name = request.Name;
			person.DateOfBirth = request.DateOfBirth;

			if (person.Address != null)                                                                                  //"pessoa com um enderço for diferente de nulo" e método dentro dele
			{                                                                                                          
				person.Address.Street = request.Address.Street;
				person.Address.Number = request.Address.Number;
				person.Address.Complement = request.Address.Complement;
				person.Address.City = request.Address.City;
				person.Address.State = request.Address.State;
				person.Address.Country = request.Address.Country;
			}


			await _repository.UpdateAsync(person);                                                              //resumo: Chamada ao método UpdateAsync do repositório para salvar as alterações da pessoa no banco de dados. O método é assíncrono, permitindo que a operação de atualização seja realizada de forma eficiente sem bloquear o thread principal.
			var response = MapToResponse(person);                                                               //resumo: Mapeamento do objeto Person atualizado para um DTO de resposta (PersonResponse) utilizando o método MapToResponse. Esse método converte os dados da pessoa atualizada em um formato adequado para ser retornado ao cliente, incluindo os detalhes da pessoa e seu endereço.
			return Ok(ApiResponse<PersonResponse>.SuccessResponse(response, "Person updated successfully"));    // Retorno do PersonResponse atualizado embrulhado no Response Padronizado (HTTP 200)
		} 



		[HttpDelete("{id}")] 
		public async Task<IActionResult> DeletePerson(int id)                                                   //resumo: Método de ação para excluir uma pessoa existente. Ele recebe um parâmetro de rota (id) e retorna um status HTTP indicando o resultado da operação.
		{
			var person = await _repository.GetByIdAsync(id);                                                    // Não usa ApiResponse pois DELETE retorna HTTP 204 (sem conteúdo/body)
			if (person == null)
			{
			 return NotFound(ApiResponse<PersonResponse>.ErrorResponse("Person not found"));                    // // Erro: pessoa não encontrada → HTTP 404 com ApiResponse padronizado
			}                                                                                                   //"DELETE retorna HTTP 204 (sem conteúdo)! Não precisa de body nem ApiResponse!"
			await _repository.DeleteAsync(id);                                                              
			return NoContent();                                                                                 // Sucesso: pessoa deletada → HTTP 204 (sem body, sem ApiResponse)
		}	


		private PersonResponse MapToResponse(Person person)                                                     //resumo: Método auxiliar para mapear um objeto Person para um DTO de resposta (PersonResponse). Ele converte os dados da pessoa, incluindo os detalhes do endereço, em um formato adequado para ser retornado ao cliente.
		{
			return new PersonResponse
			{
				Id = person.Id,
				Name = person.Name,
				DateOfBirth = person.DateOfBirth,
				Address = new AddressResponseDto
				{
					Id = person.Address.Id,
					Street = person.Address.Street,
					Number = person.Address.Number,
					Complement = person.Address.Complement,
					City = person.Address.City,
					State = person.Address.State,
					Country = person.Address.Country
				}
			};
		}
	}
}
