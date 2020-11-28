using System;

namespace Service.Models.Link.Music
{
	public class MusicDestinationModel
	{
		public Guid MediaServiceId { get; set; }
		public TrackingModel TrackingInfo { get; set; }
	}
}
