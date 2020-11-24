using System;

namespace Service.Models.StorageModel.Ticket
{
	public class DestinationStorageModel
	{
		public Guid ShowId { get; set; }
		public Guid MediaServiceId { get; set; }
		public string Url { get; set; }
		public DateTime Date { get; set; }
		public string Venue { get; set; }
		public string Location { get; set; }
		/// <summary>
		/// External Id (should not be exposed to FE)
		/// Assume that it is beeing set from another system
		/// </summary>
		public string ExternalId { get; set; }
	}
}
