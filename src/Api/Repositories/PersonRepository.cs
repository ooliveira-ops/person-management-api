using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
	public class PersonRepository : IPersonRepository                               //Implementação concreta do repositório para a entidade Person. Ele utiliza o Entity Framework Core para acessar o banco de dados e realizar as operações de CRUD definidas na interface IPersonRepository.
	{
		private readonly AppDbContext _context;

		public PersonRepository(AppDbContext context)
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


		public async Task<List<Person>> SearchAsync(string searchTerm,int pageNumber, int pageSize = 10)                //resumo: "assinatura do método" para pesquisar pessoas por um termo de busca. Ele inclui a propriedade de navegação "Address" para carregar os detalhes do endereço associado a cada pessoa e utiliza o método Where para filtrar as pessoas com base no termo de busca, verificando se o nome da pessoa ou a cidade ou estado do endereço contêm o termo de busca. Assim como no método GetAllAsync, ele também implementa a lógica de paginação usando os métodos Skip e Take para retornar apenas um subconjunto dos resultados.
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


		public async Task CreateAsync(Person person)                                                   //resumo: "assinatura do método" para criar uma nova pessoa. Ele utiliza o método AddAsync para adicionar a nova pessoa ao contexto do Entity Framework e, em seguida, chama SaveChangesAsync para salvar as alterações no banco de dados.
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
			var person = await _context.Persons	
				.Include(p => p.Address)																//"Include" = para carregar os detalhes do endereço associado à pessoa, garantindo que o endereço seja carregado junto com a pessoa para que possa ser removido corretamente.
				.FirstOrDefaultAsync(p => p.Id == id);

			if (person != null)																			//"se person for diferente de nulo"
			{
				if (person.Address != null)                                                             //se a pesoa for dif. de nulo, ou seja, se a pessoa tiver um endereço associado, o código dentro desse bloco será executado para remover o endereço do contexto do Entity Framework antes de remover a pessoa.
				{
					_context.PersonAddresses.Remove(person.Address);									//deleta o endereço primeiro
				}
				_context.Persons.Remove(person);														 //depois deleta a pessoa
				await _context.SaveChangesAsync();
			}
		}
	}
}