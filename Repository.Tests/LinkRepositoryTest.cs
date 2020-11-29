using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Tests
{
	[TestFixture]
	[ExcludeFromCodeCoverage]
	public class LinkRepositoryTest
	{
		private Context _context;
		private ILinkRepository _repository;

		[SetUp]
		public void Init()
		{
			_context = new Context();
			_repository = new LinkRepository(_context);
		}

		[TearDown]
		public void Cleanup()
		{
			_context.Dispose();
		}

		[Test]
		public void CreateLink_DomainExist_OneArtistExist()
		{
			var domain = Builder<Domain>.CreateNew()
				.With(d => d.Id, Guid.NewGuid())
				.With(d => d.Name, "domain")
				.Build();
			var artists = Builder<Artist>.CreateListOfSize(2)
				.All()
				.Do(x => x.Id = Guid.NewGuid())
				.Build().ToList();

			// imitates existing entries
			using (var contex = new Context())
			{
				contex.Domains.Add(domain);
				contex.Artists.Add(artists.First());
				contex.SaveChanges();
			}

			var link = Builder<Link>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.Artists, artists)
				.With(x => x.IsActive, true)
				.Build();

			var entity = _repository.CreateLink(link);

			var saved = _context.Links.Find(entity.Id);

			Assert.IsTrue(entity.IsActive);

			Assert.IsNotNull(saved);
			Assert.AreEqual(link.Artists.Count, saved.Artists.Count);

		}

		[Test]
		public void CreateLink_DomainExist_ArtistsAreNotExists()
		{
			var domain = Builder<Domain>.CreateNew()
				.With(d => d.Id, Guid.NewGuid())
				.With(d => d.Name, "domain")
				.Build();

			var artists = Builder<Artist>.CreateListOfSize(2)
				.All()
				.Do(x => x.Id = Guid.NewGuid())
				.Build()
				.ToList();

			// imitates existing entries
			using (var context = new Context())
			{
				context.Domains.Add(domain);
				context.SaveChanges();
			}

			var link = Builder<Link>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.Artists, artists)
				.With(x => x.IsActive, true) // this simulates the object instantiation since we are only supposed to call it using its constructor
				.Build();

			var entity = _repository.CreateLink(link);

			var saved = _context.Links.Find(entity.Id);

			Assert.IsTrue(entity.IsActive);

			Assert.IsNotNull(saved);
			Assert.AreEqual(link.Artists.Count, saved.Artists.Count);
		}


		[Test]
		public void DeleteLink_LinkExists_SuccessfulDelete()
		{
			var domain = Builder<Domain>.CreateNew()
				.With(d => d.Id, Guid.NewGuid())
				.With(d => d.Name, "domain")
				.Build();

			var link = Builder<Link>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.Title, "Test")
				.With(x => x.IsActive, true)
				.Build();

			using (var context = new Context())
			{
				context.Domains.Add(domain);
				context.Links.Add(link);
				context.SaveChanges();
			}

			var entity = _repository.DeleteLink(link.Id);

			var saved = _context.Links.Find(entity.Id);

			Assert.IsFalse(entity.IsActive);
			Assert.IsNotNull(saved);
		}

		[Test]
		public void DeleteLink_LinkDoesNotExist_ThrowsException()
		{
			Assert.Throws<Exception>(() => _repository.DeleteLink(Guid.NewGuid()));
		}

		[Test]
		public void DeleteLink_LinkIsNotActive_ThrowsException()
		{
			var domain = Builder<Domain>.CreateNew()
				.With(d => d.Id, Guid.NewGuid())
				.With(d => d.Name, "domain")
				.Build();

			var link = Builder<Link>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.Title, "Test")
				.With(x => x.IsActive, false)
				.Build();

			using (var context = new Context())
			{
				context.Domains.Add(domain);
				context.Links.Add(link);
				context.SaveChanges();
			}

			Assert.Throws<Exception>(() => _repository.DeleteLink(link.Id));
		}


		// Running this test would mean that DbEntityEntry would have to be somehow mocked. I've commented it because testing EF is out of scope for this assignment

		//[Test]
		//public void UpdateLink_LinkExists_SuccessfulUpdate() 
		//{
		//	var domain = Builder<Domain>.CreateNew()
		//		.With(d => d.Id, Guid.NewGuid())
		//		.With(d => d.Name, "domain")
		//		.Build();

		//	var link = Builder<Link>.CreateNew()
		//		.With(x => x.Id, Guid.NewGuid())
		//		.With(x => x.Domain, domain)
		//		.With(x => x.DomainId, domain.Id)
		//		.With(x => x.Title, "Test")
		//		.With(x => x.IsActive, true)
		//		.Build();

		//	using (var context = new Context())
		//	{
		//		context.Domains.Add(domain);
		//		context.Links.Add(link);
		//		context.SaveChanges();
		//	}

		//	var updatedLink = Builder<Link>.CreateNew()
		//		.With(x => x.Id, link.Id)
		//		.With(x => x.Domain, domain)
		//		.With(x => x.DomainId, domain.Id)
		//		.With(x => x.Title, "Test2")
		//		.Build();


		//	var entity = _repository.UpdateLink(link);

		//	var saved = _context.Links.Find(entity.Id);

		//	Assert.IsTrue(entity.IsActive);
		//	Assert.IsNotNull(saved);
		//	Assert.AreNotEqual(link.Title, saved.Title);

		//}
	}
}
