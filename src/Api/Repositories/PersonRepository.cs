using person_management_api.Data;
using person_management_api.Models;
using Microsoft.EntityFrameworkCore;


namespace person_management_api.Repositories
{
	public class PersonRepository : IPersonRepository                               //Implementação concreta do repositório para a entidade Person. Ele utiliza o Entity Framework Core para acessar o banco de dados e realizar as operações de CRUD definidas na interface IPersonRepository.
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


		public async Task<List<Person>> GetAllAsync(int pageNumber = 1, int pageSize = 10)              //"assinatura do método" para obter todas as pessoas com paginação. Ele inclui a propriedade de navegação "Address" para carregar os detalhes do endereço associado a cada pessoa e utiliza os métodos Skip e Take para implementar a lógica de paginação, retornando apenas um subconjunto dos resultados com base no número da página e no tamanho da página especificados.
		{
			return await _context.Persons
			.Include(p => p.Address)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
		}

		public async Task<List<Person>> SearchAsync(string searchTerm,int pageNumber)                   // resumo: "assinatura do método" para pesquisar pessoas por um termo de busca. Ele inclui a propriedade de navegação "Address" para carregar os detalhes do endereço associado a cada pessoa e utiliza o método Where para filtrar as pessoas com base no termo de busca, verificando se o nome da pessoa ou a cidade ou estado do endereço contêm o termo de busca. Assim como no método GetAllAsync, ele também implementa a lógica de paginação usando os métodos Skip e Take.
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

		public async Task CreacteAsync(Person person)                                                   //resumo: "assinatura do método" para criar uma nova pessoa. Ele utiliza o método AddAsync para adicionar a nova pessoa ao contexto do Entity Framework e, em seguida, chama SaveChangesAsync para salvar as alterações no banco de dados.
		{
			await _context.Persons.AddAsync(person);
			await _context.SaveChangesAsync();
		}
		
		public async Task UpdateAsync(Person person)                                                    //resumo: "assinatura do método" para atualizar uma pessoa existente. Ele utiliza o método Update para marcar a pessoa como modificada no contexto do Entity Framework e, em seguida, chama SaveChangesAsync para salvar as alterações no banco de dados.
		{
			_context.Persons.Update(person);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
  			var person = await _context.Persons.FindAsync(id);                                            //resumo: "assinatura do método" para excluir uma pessoa por ID. Ele utiliza o método FindAsync para localizar a pessoa no banco de dados com base no ID fornecido. Se a pessoa for encontrada, ela é removida do contexto usando o método Remove, e as alterações são salvas no banco de dados chamando SaveChangesAsync.
			if (person != null)
			{
				_context.Persons.Remove(person);
				await _context.SaveChangesAsync();
			}
		}
	}
}