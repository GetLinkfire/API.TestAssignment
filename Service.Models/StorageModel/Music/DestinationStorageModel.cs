using System;

namespace Service.Models.StorageModel.Music
{
	public class DestinationStorageModel
	{
		public Guid MediaServiceId { get; set; }
		public TrackingStorageModel TrackingInfo { get; set; }
	}
}
