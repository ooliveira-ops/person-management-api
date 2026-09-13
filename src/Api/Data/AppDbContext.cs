using System;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
	// Ponte entre as classes de modelo (Person, PersonAddress) e as tabelas do banco.
	// Herdar de DbContext é o que dá à classe a capacidade de consultar e gravar via EF Core.
	public class AppDbContext : DbContext
	{
		// As opções (provider, connection string) são montadas no Program.cs e injetadas aqui.
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		// Cada DbSet representa uma tabela: o nome da propriedade vira o nome da tabela,
		// e a classe entre <> define as colunas.
		public DbSet<Person> Persons { get; set; }
		public DbSet<PersonAddress> PersonAddresses { get; set; }

		// Chamado pelo EF Core ao construir o modelo. É aqui que se configura o que as
		// classes sozinhas não expressam: relacionamentos, chaves e comportamento de exclusão.
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Relacionamento 1:1 entre Person e PersonAddress.
			//
			// DECISÃO DE MODELAGEM (não mude sem entender o efeito):
			// A FK mora em PersonAddress, e não em Person. No EF Core, quem carrega a chave
			// estrangeira é o lado DEPENDENTE; o outro lado é o PRINCIPAL. Como um endereço
			// só faz sentido se houver uma pessoa, Person é o principal e PersonAddress o
			// dependente. Isso faz o Cascade correr na direção certa: apagar a pessoa apaga
			// o endereço, e apagar o endereço não afeta a pessoa.
			//
			// Antes da migration FixPersonAddressCascadeDelete a FK estava em Person
			// (HasForeignKey<Person>), o que invertia tudo: apagar um endereço apagava a
			// pessoa, e era impossível cadastrar alguém sem endereço.
			modelBuilder.Entity<Person>()
				.HasOne(p => p.Address)						// lado Person: tem um endereço
				.WithOne(a => a.Person)						// lado PersonAddress: pertence a uma pessoa
				.HasForeignKey<PersonAddress>(a => a.PersonId)	// a FK fica no dependente
				.OnDelete(DeleteBehavior.Cascade);			// apagar a pessoa apaga o endereço

			// O banco gera o Id do endereço (coluna IDENTITY) a cada INSERT.
			modelBuilder.Entity<PersonAddress>()
			   .Property(p => p.Id)
			   .ValueGeneratedOnAdd();
		}
	}
}
