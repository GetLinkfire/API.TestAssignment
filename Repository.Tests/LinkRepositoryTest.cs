using FizzWare.NBuilder;
using NUnit.Framework;
using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
				.Build().ToList();

			// imitates existing entries
			using (var contex = new Context())
			{
				contex.Domains.Add(domain);
				contex.SaveChanges();
			}

			var link = Builder<Link>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.Artists, artists)
				.Build();

			var entity = _repository.CreateLink(link);

			var saved = _context.Links.Find(entity.Id);

			Assert.IsTrue(entity.IsActive);

			Assert.IsNotNull(saved);
			Assert.AreEqual(link.Artists.Count, saved.Artists.Count);
		}

		[Test]
		public void UpdateLink_Success()
		{
			Domain domain = Builder<Domain>.CreateNew()
				.With(d => d.Id, Guid.NewGuid())
				.With(d => d.Name, "domain")
				.Build();

			List<Artist> artists = Builder<Artist>.CreateListOfSize(2)
				.All()
				.Do(x => x.Id = Guid.NewGuid())
				.Build().ToList();

			Link oldLink = Builder<Link>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Code, "test")
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.Artists, artists)
				.With(x => x.MediaType, MediaType.Music)
				.With(x => x.IsActive, true)
				.Build();

			// imitates existing entries
			using (Context contex = new Context())
			{
				contex.Domains.Add(domain);
				contex.Links.Add(oldLink);
				contex.SaveChanges();
			}

			Link updatedLink = _repository.GetLink(oldLink.Id);
			updatedLink.Code = "updated";

			Link entity = _repository.UpdateLink(updatedLink);

			Link saved = _context.Links.Find(entity.Id);

			Assert.IsTrue(entity.IsActive);
			Assert.IsNotNull(saved);
			Assert.AreEqual("test", oldLink.Code);
			Assert.AreEqual("updated", updatedLink.Code);
			Assert.AreEqual("updated", entity.Code);
			Assert.AreEqual(entity.Code, saved.Code);
		}

		// TODO: implement more DB link update unit tests
	}
}