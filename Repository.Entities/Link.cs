using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Entities.Enums;

namespace Repository.Entities
{
	[Table("Links")]
	public class Link
	{
		public Guid Id { get; set; }

		[Index("IX_Links_Title_Code_IsActive", 1)]
		[StringLength(255)]
		public string Title { get; set; }

		[StringLength(100)]
		[Index("IX_Links_Code")]
		[Index("IX_Links_Title_Code_IsActive", 2)]
		public string Code { get; set; }

		[Index("IX_Links_Title_Code_IsActive", 3)]
		public bool IsActive { get; set; }

		public string Url { get; set; }

		public MediaType MediaType { get; set; }

		public Guid DomainId { get; set; }

		[ForeignKey("DomainId")]
		public virtual Domain Domain { get; set; }

		public virtual ICollection<Artist> Artists { get; set; }
	}
}
