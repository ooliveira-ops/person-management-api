using System;


namespace Api.Models
{
	public class PersonAddress                                          //(2) Classe de modelo para a entidade PersonAddress, que representa o endereço de uma pessoa. Ela inclui as propriedades Id, Street, Number, Complement, City, State e Country. Essa classe é usada para armazenar os detalhes do endereço associado a uma pessoa na tabela PersonAddress do banco de dados.
	{
		public int Id { get; set; }
		public string Street { get; set; }
		public string Number { get; set; }
		public string Complement { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
	}	
}
