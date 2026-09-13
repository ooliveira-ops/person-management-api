using System;

namespace Api.Models
{
	// Entidade PersonAddress — mapeada para a tabela PersonAddresses.
	// É o lado DEPENDENTE do relacionamento 1:1: guarda a FK para Person e é apagada
	// em cascata quando a pessoa é removida. Ver AppDbContext.OnModelCreating.
	public class PersonAddress
	{
		public int Id { get; set; }

		// FK para Persons.Id. Obrigatória: um endereço sempre pertence a alguém.
		public int PersonId { get; set; }
		public Person? Person { get; set; }

		public string Street { get; set; }
		public string Number { get; set; }

		// Único campo opcional do endereço. O "?" não é decorativo: é ele que faz o EF
		// gerar a coluna como NULL. Enquanto era "string", o banco exigia NOT NULL e um
		// POST sem complement quebrava com 500, mesmo o DTO tratando o campo como opcional.
		public string? Complement { get; set; }

		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
	}
}
