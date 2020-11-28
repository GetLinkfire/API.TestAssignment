using System;

namespace Service.Models
{
	public class ArtistModel
	{
		public Guid Id { get; set; } // TODO: On link creation this is being sent by the client, mapped and created in the database
		public string Name { get; set; }
		public string Label { get; set; }
	}
}
