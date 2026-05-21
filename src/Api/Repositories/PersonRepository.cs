using person_management_api.Data;
using person_management_api.Models;
using Microsoft.EntityFrameworkCore;


namespace person_management_api.Repositories
{
	public class PersonRepository : IPersonRepository
	{
		private readonly AppDbContext _context;

		public Person PersonRepository(AppDbContext context)
		{
			_context = context;                                                 //construtor, que ja vai saberfalar com o database(context)
		}

		public async Task<Person> GetByIdAsync(int id)                                          //"assinatura do método"
		{
			return await _context.Persons
			.Include(p => p.Address)
			.FirstOrDefaultAsync(p => p.Id == id);
		}


		public async Task<List<Person>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
		{
			return await _context.Persons
			.Include(p => p.Address)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
		}

		public async Task<List<Person>> SearchAsync(string searchTerm,int pageNumber)
		{
			return await _context.Persons
			.Include(p => p.Address)
			.Where(p => p.Name.Contains(searchTerm) ||
						p.Address.City.Contains(searchTerm) ||
						p.Address.State.Contains(searchTerm))
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
		}

		public async Task CreacteAsync(Person person)
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
  			var person = await _context.Persons.FindAsync(id);
			if (person != null)
			{
				_context.Persons.Remove(person);
				await _context.SaveChangesAsync();
			}
		}
	}
}