using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.Entities
{
	public class Artist
	{
		public Guid Id { get; set; }
		[StringLength(255)]
		public string Name { get; set; }
		
		[StringLength(255)]
		public string Label { get; set; }
		
		public virtual ICollection<Link> Links { get; set; }
	}
}
