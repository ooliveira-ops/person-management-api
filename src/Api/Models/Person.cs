using System;

namespace Api.Models
{
	// Entidade Person — mapeada para a tabela Persons.
	// É o lado PRINCIPAL do relacionamento 1:1 com PersonAddress: a pessoa existe por si só,
	// e por isso não guarda FK nenhuma. Quem aponta para quem é o endereço.
	public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }

		// Propriedade de navegação: dá acesso ao endereço a partir da pessoa.
		// Anulável porque o endereço pode não ter sido carregado (consulta sem Include)
		// ou simplesmente não existir. Sempre cheque antes de usar.
		public PersonAddress? Address { get; set; }
	}
}
