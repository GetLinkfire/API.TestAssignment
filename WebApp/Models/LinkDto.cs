using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Repository.Entities.Enums;
using Service.Models;

namespace WebApp.Models
{
	public class CreateLinkDto
	{
		[Required]
		[StringLength(255)]
		public string Title { get; set; }

		[StringLength(100, MinimumLength = 2)]
		[RegularExpression("^(?=.*[a-zA-Z0-9])([a-zA-Z0-9-_]+)$")]
		public string Code { get; set; }

		[Required]
		public Guid DomainId { get; set; }

		[Required]
		[Url]
		public string Url { get; set; }

		[Required]
		public MediaType MediaType { get; set; }

		public List<ArtistModel> Artists { get; set; }

		public List<MusicDestinationDto> MusicDestinations { get; set; }

		public List<TicketDestinationDto> TicketDestinations { get; set; }
	}

	public class LinkDto : CreateLinkDto
	{
		public Guid Id { get; set; }

		public string Web { get; set; }
		public string Mobile { get; set; }

		public string Artist { get; set; }
		public string Album { get; set; }
		public string SongTitle { get; set; }
	}
}