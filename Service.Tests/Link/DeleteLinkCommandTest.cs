using System;
using System.Diagnostics.CodeAnalysis;
using FizzWare.NBuilder;
using NUnit.Framework;
using Repository.Entities;
using Repository.Interfaces;
using Rhino.Mocks;
using Service.Interfaces.Storage;
using Service.Links;

namespace Service.Tests.Link
{
	[TestFixture]
	[ExcludeFromCodeCoverage]
	public class DeleteLinkCommandTest
	{
		private IStorage _storageService;
		private ILinkRepository _linkRepository;
		private IDomainRepository _domainRepository;
		private IMediaServiceRepository _mediaServiceRepository;
		private DeleteLinkCommand _deleteLinkCommand;

		[SetUp]
		public void Init()
		{
			_linkRepository = MockRepository.GenerateMock<ILinkRepository>();
			_domainRepository = MockRepository.GenerateMock<IDomainRepository>();
			_mediaServiceRepository = MockRepository.GenerateMock<IMediaServiceRepository>();
			_storageService = MockRepository.GenerateMock<IStorage>();
			_deleteLinkCommand = new DeleteLinkCommand(_storageService, _domainRepository, _mediaServiceRepository, _linkRepository);

		}

		[TearDown]
		public void VerifyAllExpectations()
		{
			_storageService.VerifyAllExpectations();
			_linkRepository.VerifyAllExpectations();
			_domainRepository.VerifyAllExpectations();
			_mediaServiceRepository.VerifyAllExpectations();
		}

		[Test]
		public void Execute_ExistingLink_SuccessfulDelete()
		{
			var request = Builder<DeleteLink>
				.CreateNew()
				.With(x => x.LinkId, Guid.NewGuid())
				.Build();

			var domain = Builder<Domain>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Name, "mydomain.com")
				.Build();

			var deletedLink = Builder<Repository.Entities.Link>
				.CreateNew()
				.With(x => x.Id, request.LinkId)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.IsActive, false)
				.Build();

			_linkRepository.Expect(x => x.DeleteLink(Arg<Guid>.Is.Equal(request.LinkId))).Return(deletedLink);
			_domainRepository.Expect(x => x.GetDomain(Arg<Guid>.Is.Equal(domain.Id))).Return(domain);

			_deleteLinkCommand.Execute(request);
		}
	}
}
