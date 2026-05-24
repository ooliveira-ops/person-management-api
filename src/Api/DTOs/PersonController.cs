using System;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<ActionResult<PersonResponse>> CreatePerson(CreatePersonRequest request)                   //resumo: Método de ação para criar uma nova pessoa. Ele recebe um DTO de solicitação (CreatePersonRequest) contendo os dados necessários para criar uma pessoa, e retorna um DTO de resposta (PersonResponse) com os detalhes da pessoa criada.
		{
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
			return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, response);
		}




		[HttpGet("{id}")]
		public async Task<ActionResult<PersonResponse>> GetPersonById(int id)                                   //resumo: Método de ação para obter os detalhes de uma pessoa com base em seu ID. Ele recebe um parâmetro de rota (id) e retorna um DTO de resposta (PersonResponse) contendo os detalhes da pessoa correspondente ao ID fornecido.
		{
			var person = await _repository.GetByIdAsync(id);                                                    //resumo: Chamada ao método GetByIdAsync do repositório para recuperar a pessoa do banco de dados com base no ID fornecido. O método é assíncrono, permitindo que a operação de recuperação seja realizada de forma eficiente sem bloquear o thread principal.
			if (person == null)
			{
				return NotFound(new { message = "Person not found" });                                          //resumo: Verificação se a pessoa foi encontrada. Se o objeto person for nulo, significa que não existe uma pessoa com o ID fornecido, e o método retorna um status HTTP 404 Not Found com a mensagem de não encotarda
			}
			var response = MapToResponse(person);                                                                //resumo: Mapeamento do objeto Person para um DTO de resposta (PersonResponse) utilizando o método MapToResponse. Esse método converte os dados da pessoa recuperada em um formato adequado para ser retornado ao cliente, incluindo os detalhes da pessoa e seu endereço.
			return Ok(response);                                                                                //resumo: Retorno do DTO de resposta com um status HTTP 200 OK, indicando que a operação foi bem-sucedida e os detalhes da pessoa foram retornados ao cliente.
		}



		[HttpGet]
		public async Task<ActionResult<List<PersonResponse>>> GetPersons([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)     //resumo: Método de ação para obter uma lista de pessoas com suporte à paginação e filtragem por nome. Ele recebe parâmetros de consulta (page, pageSize e name) e retorna uma lista de DTOs de resposta (PersonResponse) contendo os detalhes das pessoas que correspondem aos critérios fornecidos.
		{
			List<Person> persons;                                                                                                   //resumo: Declaração de uma variável para armazenar a lista de pessoas recuperadas do banco de dados. A lista será preenchida com os resultados da consulta realizada no repositório, levando em consideração os parâmetros de paginação e filtragem fornecidos.

			if (!string.IsNullOrEmpty(search))                                                                                  //resumo: Verificação se o parâmetro de pesquisa (search) foi fornecido. Se o parâmetro não for nulo ou vazio, significa que o cliente deseja filtrar as pessoas com base no nome, e a consulta será realizada utilizando o método GetByNameAsync do repositório.
			{
				persons = await _repository.SearchAsync(search, page, pageSize);                                             //resumo: Chamada ao método SearchAsync do repositório para recuperar as pessoas do banco de dados que correspondem ao critério de pesquisa
			}
			else
			{
				persons = await _repository.GetAllAsync(page, pageSize);                                                           //resumo: Chamada ao método GetAllAsync do repositório para recuperar todas as pessoas do banco de dados, aplicando a paginação com base nos parâmetros fornecidos.
			}

			var response = persons.Select(MapToResponse).ToList();                                                               //resumo: Mapeamento da lista de objetos Person para uma lista de DTOs de resposta (PersonResponse) utilizando o método MapToResponse. Esse método converte os dados das pessoas recuperadas em um formato adequado para ser retornado ao cliente, incluindo os detalhes das pessoas e seus endereços.
			return Ok(response);                                                                                                 //resumo: Retorno da lista de DTOs de resposta com um status HTTP 200 OK, indicando que a operação foi bem-sucedida e os detalhes das pessoas foram retornados ao cliente.
		}



		[HttpPut("{id}")]
		public async Task<ActionResult<PersonResponse>> UpdatePerson(int id, UpdatePersonRequest request)                   //resumo: Método de ação para atualizar os detalhes de uma pessoa existente. Ele recebe um parâmetro de rota (id) e um DTO de solicitação (UpdatePersonRequest) contendo os dados atualizados da pessoa, e retorna um DTO de resposta (PersonResponse) com os detalhes da pessoa atualizada.
		{
			var person = await _repository.GetByIdAsync(id);                                                    //resumo: Chamada ao método GetByIdAsync do repositório para recuperar a pessoa do banco de dados com base no ID fornecido. O método é assíncrono, permitindo que a operação de recuperação seja realizada de forma eficiente sem bloquear o thread principal.
			if (person == null)
			{
				return NotFound(new { message = "Person not found" });                                          //resumo: Verificação se a pessoa foi encontrada. Se o objeto person for nulo, significa que não existe uma pessoa com o ID fornecido, e o método retorna um status HTTP 404 Not Found com a mensagem de não encontrada.
			}

			person.Name = request.Name;
			person.DateOfBirth = request.DateOfBirth;

			if (person.Address != null)                                                                                  //resumo: Verificação se a pessoa possui um endereço associado. Se a propriedade Address for nula ou vazia, significa que a pessoa não tem um endereço registrado, e um novo objeto PersonAddress é criado utilizando os dados fornecidos no DTO de solicitação.
			{ 
				person.Address.Street = request.Address.Street;
				person.Address.Number = request.Address.Number;
				person.Address.Complement = request.Address.Complement;
				person.Address.City = request.Address.City;
				person.Address.State = request.Address.State;
				person.Address.Country = request.Address.Country;
			}


			await _repository.UpdateAsync(person);                                                              //resumo: Chamada ao método UpdateAsync do repositório para salvar as alterações da pessoa no banco de dados. O método é assíncrono, permitindo que a operação de atualização seja realizada de forma eficiente sem bloquear o thread principal.
			var response = MapToResponse(person);																//resumo: Mapeamento do objeto Person atualizado para um DTO de resposta (PersonResponse) utilizando o método MapToResponse. Esse método converte os dados da pessoa atualizada em um formato adequado para ser retornado ao cliente, incluindo os detalhes da pessoa e seu endereço.
			return Ok(response);                                                                                //resumo: Retorno do DTO
		} 



		[HttpDelete("{id}")] 
		public async Task<IActionResult> DeletePerson(int id)                                                   //resumo: Método de ação para excluir uma pessoa existente. Ele recebe um parâmetro de rota (id) e retorna um status HTTP indicando o resultado da operação.
		{
			var person = await _repository.GetByIdAsync(id);                                                    //resumo: Chamada ao método GetByIdAsync do repositório para recuperar a pessoa do banco de dados com base no ID fornecido. O método é assíncrono, permitindo que a operação de recuperação seja realizada de forma eficiente sem bloquear o thread principal.
			if (person == null)
			{
			return NotFound(new { message = "Person not found" });                                          
			}
			await _repository.DeleteAsync(id);                                                              
			return NoContent();                                                                                 //resumo: Retorno de um status HTTP 204 No Content(não há conteúdo), indicando que a operação foi bem-sucedida e a pessoa foi excluída do sistema.
		}


		private PersonResponse MapToResponse(Person person)
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
