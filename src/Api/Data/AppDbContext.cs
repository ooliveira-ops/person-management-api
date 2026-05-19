using System;
using Microsoft.EntityFrameworkCore;                   //importações//C# : "Vou usar o Entity Framework Core para criar um contexto de banco de dados"
using Api.Models;                                   //importações//C# : "Vou usar as classes de modelo Person e PersonAddress(em Api.Models) para definir as entidades do banco de dados"

namespace Api.Data
{
	public class AppDbContext : DbContext													//recebendo "poderes" para falar com o banco de dados
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)         //Dbcon.. options = recebe as conf / base(options) = passa as conf para a classe base (DbContext)
		{
		}

		public DbSet<Person> Persons { get; set; }										//db set = representa as tabelas do banco de dados, "Persons" é o nome da tabela, "Person" é a classe de modelo que define a estrutura dos dados nessa tabela
		public DbSet<PersonAddress> PersonAddresses { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)              //metodo para configurar o modelo de dados, onde podemos definir relacionamentos, chaves primárias, etc. Ele é chamado quando o modelo de dados está sendo criado e nos permite personalizar a estrutura do banco de dados gerado pelo Entity Framework
		{

			base.OnModelCreating(modelBuilder);

			//configura relacionamento: "Person tem UM PersonAddress, e PersonAddress tem UM Person"
			modelBuilder.Entity<Person>()														//configurações adicionais para a entidade "Person"
				.HasOne(p => p.Address)															//configura o relacionamento 1 para 1 entre Person e PersonAddress, indicando que cada Person tem um Address
				.WithOne()																		//configura o outro lado do relacionamento, indicando que cada PersonAddress está associado a um único Person
				.HasForeignKey<Person>(p => p.AddressId)										//configura a chave estrangeira para o relacionamento, indicando que a propriedade "AddressId" em Person é a chave estrangeira que referencia a tabela PersonAddress
				.OnDelete(DeleteBehavior.Cascade);												//configura o comportamento de exclusão em cascata, indicando que quando um registro de Person for excluído, o registro correspondente em PersonAddress também será excluído automaticamente
		}
	}
}
