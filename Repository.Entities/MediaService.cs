using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities
{
	[Table("MediaServices")]
	public class MediaService : BaseEntity
	{
		public string Name { get; set; }
	}
}
