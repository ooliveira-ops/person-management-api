using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
	// Implementação de IPersonRepository sobre o EF Core. É a única camada que fala
	// com o banco: o controller nunca toca no DbContext diretamente.
	public class PersonRepository : IPersonRepository
	{
		private readonly AppDbContext _context;

		// Teto de itens por página. Sem ele, um pageSize grande faria o banco materializar
		// a tabela inteira numa única requisição. Vale para GetAllAsync e SearchAsync.
		private const int MaxPageSize = 100;

		public PersonRepository(AppDbContext context)
		{
			_context = context;
		}

		// O Include é obrigatório: sem ele o EF devolve a pessoa com Address nulo,
		// e a resposta da API sairia sem endereço.
		public async Task<Person> GetByIdAsync(int id)
		{
			return await _context.Persons
			.Include(p => p.Address)
			.FirstOrDefaultAsync(p => p.Id == id);
		}

		// Lista paginada. Skip/Take traduzem para OFFSET/FETCH no SQL Server.
		public async Task<List<Person>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
		{
			// Normaliza ANTES do Skip: com pageNumber = 0 o cálculo daria Skip(-10),
			// e o SQL Server rejeita OFFSET negativo com erro (resultando em 500 na API).
			if (pageNumber < 1) pageNumber = 1;
			pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

			 return await _context.Persons
				.Include(p => p.Address)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}

		// Busca o termo em três campos ao mesmo tempo: nome da pessoa, cidade e estado.
		// Atenção: Contains é case-sensitive no SQLite (usado nos testes) e
		// case-insensitive no SQL Server — o mesmo código se comporta diferente nos dois.
		public async Task<List<Person>> SearchAsync(string searchTerm,int pageNumber, int pageSize = 10)
		{
			// Mesma normalização do GetAllAsync — o Skip abaixo nunca pode ficar negativo.
			if (pageNumber < 1) pageNumber = 1;
			pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

			return await _context.Persons
			.Include(p => p.Address)
			.Where(p => p.Name.Contains(searchTerm) ||
						p.Address.City.Contains(searchTerm) ||
						p.Address.State.Contains(searchTerm))
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
		}

		// Grava a pessoa e, junto, o endereço associado: o EF percorre a navegação
		// Address e insere as duas linhas na mesma transação.
		public async Task CreateAsync(Person person)
		{
			await _context.Persons.AddAsync(person);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Person person)
		{
			_context.Persons.Update(person);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var person = await _context.Persons
				.Include(p => p.Address)
				.FirstOrDefaultAsync(p => p.Id == id);

			// Id inexistente não é erro: a operação simplesmente não faz nada,
			// e o controller devolve 404 por conta própria.
			if (person != null)
			{
				// Remoção explícita do endereço. Hoje é redundante — a FK em
				// PersonAddresses.PersonId tem ON DELETE CASCADE e o banco faria isso
				// sozinho —, mas é inofensiva e mantém o comportamento explícito.
				if (person.Address != null)
				{
					_context.PersonAddresses.Remove(person.Address);
				}
				_context.Persons.Remove(person);
				await _context.SaveChangesAsync();
			}
		}
	}
}
