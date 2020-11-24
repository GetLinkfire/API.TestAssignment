namespace Repository.Entities.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	public sealed class Configuration : DbMigrationsConfiguration<Repository.Entities.Context>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(Repository.Entities.Context context)
		{
			context.Domains.AddOrUpdate(
				new Domain() { Id = Guid.Parse("429bdd73-25a7-4b98-8e3a-6617fd66712b"), Name = "lnk.to" },
				new Domain() { Id = Guid.Parse("32023fb0-01b1-4f77-9ae9-1face902b6c6"), Name = "tix.to" }
			);

			context.MediaServices.AddOrUpdate(
				new MediaService() { Id = Guid.Parse("44264302-9556-47e2-92b9-443b466ceb2f"), Name = "spotify" },
				new MediaService() { Id = Guid.Parse("1fb91374-8fca-44b7-b113-8a856d223ca3"), Name = "deezer" },
				new MediaService() { Id = Guid.Parse("4b5cccf1-1d89-4beb-ad03-f84ebf666e3f"), Name = "iTunes" }
			);

		}
	}
}
