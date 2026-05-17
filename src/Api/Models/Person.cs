using System;

namespace Api.Models
{
	public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }

		//Relacionamento com PersonAddress
		public int AddressId { get; set; }
		public PersonAddress Address { get; set; }
	}
}
