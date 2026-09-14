using System;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Api.Response;
using FluentValidation;

namespace Api.Controllers
{
	// Endpoints REST de Person. O controller não conhece o banco: recebe DTOs, converte
	// para entidades, delega ao repositório e devolve tudo embrulhado em ApiResponse.
	//
	// [ApiController] valida as Data Annotations dos DTOs automaticamente e devolve 400
	// antes de o método executar — por isso não há checagem manual de campos obrigatórios.
	[ApiController]
	[Route("api/[controller]")]
	public class PersonsController : ControllerBase
	{
		private readonly IPersonRepository _repository;
		private readonly IValidator<Person> _validator;
		private readonly ILogger<PersonsController> _logger;

		// Repositório, validador e logger chegam prontos pela injeção de dependência
		// (Program.cs). Construtor único de propósito: um segundo construtor deixaria
		// o contêiner de DI ambíguo e permitiria instâncias com campos nulos.
		public PersonsController(
			IPersonRepository repository,
			IValidator<Person> validator,
			ILogger<PersonsController> logger)
		{
			_repository = repository;
			_validator = validator;
			_logger = logger;
		}

		// Roda as regras do PersonValidator sobre a entidade e devolve as mensagens de erro
		// agregadas, ou null se estiver tudo certo.
		//
		// Centralizar aqui garante que POST e PUT apliquem exatamente as mesmas regras:
		// antes, a checagem de data futura era feita à mão só no POST, e o PUT aceitava
		// uma data de nascimento no futuro.
		private async Task<string?> ValidateAsync(Person person)
		{
			var result = await _validator.ValidateAsync(person);
			if (result.IsValid)
			{
				return null;
			}
			return string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
		}

		[HttpPost]
		public async Task<ActionResult<ApiResponse<PersonResponse>>> CreatePerson(CreatePersonRequest request)
		{
			var address = new PersonAddress
			{
				Street = request.Address.Street,
				Number = request.Address.Number,
				Complement = request.Address.Complement,	// pode ser null: campo opcional
				City = request.Address.City,
				State = request.Address.State,
				Country = request.Address.Country
			};

			var person = new Person
			{
				Name = request.Name,
				DateOfBirth = request.DateOfBirth,
				Address = address		// o EF grava pessoa e endereço na mesma operação
			};

			// Regras de negócio (data não futura, tamanho do nome) antes de tocar no banco.
			var erro = await ValidateAsync(person);
			if (erro != null)
			{
				_logger.LogWarning("Criação recusada pela validação: {Erro}", erro);
				return BadRequest(ApiResponse<PersonResponse>.ErrorResponse(erro));
			}

			await _repository.CreateAsync(person);
			_logger.LogInformation("Pessoa {PersonId} criada", person.Id);

			var response = MapToResponse(person);
			// CreatedAtAction devolve 201 e inclui no header Location a URL do novo recurso.
			return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, ApiResponse<PersonResponse>.SuccessResponse(response, "Person created successfully"));
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<PersonResponse>>> GetPersonById(int id)
		{
			var person = await _repository.GetByIdAsync(id);
			if (person == null)
			{
				_logger.LogWarning("Pessoa {PersonId} não encontrada", id);
				return NotFound(ApiResponse<PersonResponse>.ErrorResponse("Person not found"));
			}
			var response = MapToResponse(person);
			return Ok(ApiResponse<PersonResponse>.SuccessResponse(response));
		}


		// Lista paginada, com busca opcional por nome, cidade ou estado.
		// page e pageSize não são validados aqui: quem normaliza é o PersonRepository,
		// para que a proteção valha em qualquer chamador, não só neste endpoint.
		[HttpGet]
		public async Task<ActionResult<ApiResponse<List<PersonResponse>>>> GetPersons([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
		{
			List<Person> persons;

			if (!string.IsNullOrEmpty(search))
			{
				persons = await _repository.SearchAsync(search, page, pageSize);
			}
			else
			{
				persons = await _repository.GetAllAsync(page, pageSize);
			}

			var response = persons.Select(MapToResponse).ToList();
			return Ok(ApiResponse<List<PersonResponse>>.SuccessResponse(response));
		}


		[HttpPut("{id}")]
		public async Task<ActionResult<ApiResponse<PersonResponse>>> UpdatePerson(int id, UpdatePersonRequest request)
		{
			// Busca a entidade rastreada pelo EF e altera os campos nela: assim o
			// SaveChangesAsync do repositório gera um UPDATE só do que mudou.
			var person = await _repository.GetByIdAsync(id);
			if (person == null)
			{
				return NotFound(ApiResponse<PersonResponse>.ErrorResponse("Person not found"));
			}

			// Valida os dados recebidos ANTES de alterar a entidade rastreada pelo EF:
			// assim uma requisição inválida não deixa mudanças pendentes no contexto.
			var candidato = new Person
			{
				Name = request.Name,
				DateOfBirth = request.DateOfBirth
			};
			var erro = await ValidateAsync(candidato);
			if (erro != null)
			{
				return BadRequest(ApiResponse<PersonResponse>.ErrorResponse(erro));
			}

			person.Name = request.Name;
			person.DateOfBirth = request.DateOfBirth;

			// Os DOIS lados precisam ser checados: person.Address pode não existir no banco,
			// e request.Address pode não ter vindo no corpo da requisição.
			if (person.Address != null && request.Address != null)
			{
				person.Address.Street = request.Address.Street;
				person.Address.Number = request.Address.Number;
				person.Address.Complement = request.Address.Complement;
				person.Address.City = request.Address.City;
				person.Address.State = request.Address.State;
				person.Address.Country = request.Address.Country;
			}

			await _repository.UpdateAsync(person);
			var response = MapToResponse(person);
			return Ok(ApiResponse<PersonResponse>.SuccessResponse(response, "Person updated successfully"));
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePerson(int id)
		{
			var person = await _repository.GetByIdAsync(id);
			if (person == null)
			{
				 return NotFound(ApiResponse<PersonResponse>.ErrorResponse("Person not found"));
			}
			await _repository.DeleteAsync(id);
			_logger.LogInformation("Pessoa {PersonId} removida", id);
			// 204 é a resposta correta para DELETE bem-sucedido: sem body, logo sem ApiResponse.
			return NoContent();
		}

		// Converte a entidade do banco no DTO de saída, para não expor o modelo interno
		// (e, com ele, a navegação Person dentro de Address, que causaria ciclo no JSON).
		private PersonResponse MapToResponse(Person person)
		{
			return new PersonResponse
			{
				Id = person.Id,
				Name = person.Name,
				DateOfBirth = person.DateOfBirth,
				// Address é anulável desde que o endereço virou o lado dependente.
				// Sem este teste, uma pessoa sem endereço derrubaria a resposta com
				// NullReferenceException (500) em vez de devolver "address": null.
				Address = person.Address == null ? null : new AddressResponseDto
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
