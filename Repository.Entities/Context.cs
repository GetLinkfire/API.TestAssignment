using System.Data.Entity;

namespace Repository.Entities
{
	public class Context:DbContext
	{
		public DbSet<Link> Links { get; set; }
		public DbSet<Domain> Domains { get; set; }
		public DbSet<Artist> Artists { get; set; }
		public DbSet<MediaService> MediaServices { get; set; }
	}
}
