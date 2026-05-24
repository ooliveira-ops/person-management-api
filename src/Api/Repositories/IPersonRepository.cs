using person_management_api.Models;

namespace person_management_api.Repositories
{
	public interface IPersonRepository                                     // Interface: define o "o que" fazer de para acessar os dados relacionados à entidade Person. Ela inclui métodos para obter uma pessoa por ID, obter todas as pessoas com paginação, pesquisar pessoas por um termo de busca, criar, atualizar e excluir pessoas. A implementação concreta dessa interface será responsável por fornecer a lógica específica para acessar o banco de dados usando o Entity Framework Core ou qualquer outra tecnologia de acesso a dados.

	{
		Task<Person> GetByIdAsync(int id);
		Task<List<Person>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
		Task<List<Person>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
		Task CreateAsync(Person person);
		Task UpdateAsync(Person person);
		Task DeleteAsync(int id);
	}
}

