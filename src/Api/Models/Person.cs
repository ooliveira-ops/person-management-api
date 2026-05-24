using System;

namespace Api.Models
{
	public class Person                                                 //(1) Classe de modelo para a entidade Person, que representa uma pessoa com suas propriedades e o relacionamento com a entidade PersonAddress. Ela inclui as propriedades Id, Name, DateOfBirth, AddressId e a propriedade de navegação Address para acessar os detalhes do endereço associado a essa pessoa.
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }

		//Relacionamento com PersonAddress : 1 para 1	=		"prédio=person; Alicerce(PersonAddress); sem o alicerce, o prédio cai. pois se deleta person, o prédio também será deletado"
		public int AddressId { get; set; }						//Person tem UM Address
		public PersonAddress Address { get; set; }				//"Address" é a propriedade de navegação para acessar os detalhes do endereço associado a essa pessoa
	}
}
								