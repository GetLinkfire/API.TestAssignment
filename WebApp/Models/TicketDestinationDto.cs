using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class TicketDestinationDto : DestinationDto
	{
		[Required]
		public Guid ShowId { get; set; }

		[Required]
		public Guid MediaServiceId { get; set; }

		[Required]
		[Url]
		public string Url { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		[StringLength(255)]
		public string Venue { get; set; }

		[Required]
		[StringLength(255)]
		public string Location { get; set; }
	}
}