using person_management_api.Models;

namespace person_management_api.Repositories
{
	public interface IPersonRepository									   // Interface: define o "o que" fazer
																		   	
	{
		Task<Person> GetByIdAsync(int id);
		Task<List<Person>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
		Task<List<Person>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
		Task CreateAsync(Person person);
		Task UpdateAsync(Person person);
		Task DeleteAsync(int id);
	}
}

