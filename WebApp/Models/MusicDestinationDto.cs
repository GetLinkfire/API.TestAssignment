using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class MusicDestinationDto
	{
		[Required]
		public Guid MediaServiceId { get; set; }

		[Required]
		[StringLength(2, MinimumLength = 2)]
		public string IsoCode { get; set; }

		[Required]
		[Url]
		public string Web { get; set; }

		[Url]
		public string Mobile { get; set; }
		
		public string Artist { get; set; }
		public string Album { get; set; }
		public string SongTitle { get; set; }
	}
}