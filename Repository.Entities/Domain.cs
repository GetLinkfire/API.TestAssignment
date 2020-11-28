using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities
{
	[Table("Domains")]
	public class Domain : BaseEntity
	{
		public string Name { get; set; }
	}
}
