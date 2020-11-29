using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Entities.Enums;

namespace Repository.Entities
{
	[Table("Links")]
	public class Link : BaseEntity
	{
		public Link() { }

		public Link(string title, MediaType mediaType, string code, Domain domain, string url, ICollection<Artist> artists) : this()
		{
			Id = Guid.NewGuid();
			Title = title;
			MediaType = mediaType;
			Code = code;
			Url = url;
			Artists = artists;
			IsActive = true;
			Domain = domain;
			DomainId = domain.Id;
		}

		[Index("IX_Links_Title_Code_IsActive", 1)]
		[StringLength(255)]
		public string Title { get; set; }

		[StringLength(100)]
		[Index("IX_Links_Code")]
		[Index("IX_Links_Title_Code_IsActive", 2)]
		public string Code { get; set; }

		[Index("IX_Links_Title_Code_IsActive", 3)]
		public bool IsActive { get; private set; }

		public string Url { get; set; }

		public MediaType MediaType { get; private set; }

		public Guid DomainId { get; set; }

		[ForeignKey("DomainId")]
		public virtual Domain Domain { get; set; }

		public virtual ICollection<Artist> Artists { get; set; }

		/// <summary>
		/// Sets the link as not active
		/// </summary>
		public void SetInactive()
		{
			IsActive = false;
		}
	}
}
