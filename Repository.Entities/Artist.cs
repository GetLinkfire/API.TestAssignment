using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace Repository.Entities
{
	public class Artist : BaseEntity
	{
		public Artist() { }

		public Artist(string name, string label) 
		{
			Id = Guid.NewGuid();
			Name = name;
			Label = label;
		}

		[StringLength(255)]
		public string Name { get; set; }
		
		[StringLength(255)]
		public string Label { get; set; }
		
		public virtual ICollection<Link> Links { get; set; }
    }
}
