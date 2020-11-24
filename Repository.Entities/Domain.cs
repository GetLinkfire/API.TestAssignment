using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities
{
	[Table("Domains")]
	public class Domain
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
