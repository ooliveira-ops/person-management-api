using Api.Models;

namespace Api.Repositories
{
	// Contrato de acesso a dados de Person: define O QUE pode ser feito, sem dizer como.
	// O controller depende desta interface, não da implementação — é o que permite trocar
	// o banco por outro provider e mockar o repositório nos testes de controller.
	public interface IPersonRepository
	{
		Task<Person> GetByIdAsync(int id);
		Task<List<Person>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
		Task<List<Person>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
		Task CreateAsync(Person person);
		Task UpdateAsync(Person person);
		Task DeleteAsync(int id);
	}
}
