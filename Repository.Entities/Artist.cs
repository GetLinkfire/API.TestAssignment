using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Repository.Entities
{
	public class Artist : BaseEntity
	{
		public Artist() { }

		public Artist(string name, string label, ICollection<Link> links = null) 
		{
			Id = Guid.NewGuid();
			Name = name;
			Label = label;
			Links = links;
		}

		[StringLength(255)]
		public string Name { get; set; }
		
		[StringLength(255)]
		public string Label { get; set; }
		
		public virtual ICollection<Link> Links { get; set; }
	}
}
