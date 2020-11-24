using System;
using System.Collections.Generic;
using Repository.Entities.Enums;

namespace Service.Models.StorageModel.Ticket
{
	public class StorageModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
		public MediaType MediaType { get; set; }

		public Dictionary<string, List<DestinationStorageModel>> Destinations { get; set; }
	}
}
