using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities
{
	[Table("MediaServices")]
	public class MediaService
	{
		public Guid Id { get; set; }

		public string Name { get; set; }
	}
}
