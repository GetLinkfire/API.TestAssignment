using System;

namespace Service.Models.Link.Music
{
	public class DestinationModel
	{
		public Guid MediaServiceId { get; set; }
		public TrackingModel TrackingInfo { get; set; }
	}
}
