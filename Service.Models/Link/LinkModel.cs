using System;
using System.Collections.Generic;
using Repository.Entities.Enums;

namespace Service.Models.Link
{
    public class LinkModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Code { get; set; }
		public Guid DomainId { get; set; }		
		public List<ArtistModel> Artists { get; set; }
		public string Url { get; set; }
		public MediaType MediaType { get; set; }
		public bool IsActive { get; set; }
	}
}
