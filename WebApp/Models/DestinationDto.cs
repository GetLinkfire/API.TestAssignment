using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public abstract class DestinationDto
	{
		[Required]
		[StringLength(2, MinimumLength = 2)]
		public string IsoCode { get; set; }
	}
}