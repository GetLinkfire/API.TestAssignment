using System.Collections.Generic;
using Service.Models.Link.Music;

namespace Service.Models.Link
{
	public class ExtendedLinkModel : LinkModel
	{
		public TrackingModel TrackingInfo { get; set; }

		public Dictionary<string, List<MusicDestinationModel>> MusicDestinations { get; set; }

		public Dictionary<string, List<Ticket.TicketDestinationModel>> TicketDestinations { get; set; }
	}
}
