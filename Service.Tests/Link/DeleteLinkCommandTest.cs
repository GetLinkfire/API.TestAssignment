using FizzWare.NBuilder;
using NUnit.Framework;
using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Interfaces;
using Rhino.Mocks;
using Service.Interfaces.Storage;
using Service.Link;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Service.Tests.Link
{
    [TestFixture]
	[ExcludeFromCodeCoverage]
	public class DeleteLinkCommandTest
	{
		private ILinkRepository _linkRepository;
		private IStorage _storageService;
		private DeleteLinkCommand _deleteLinkCommand;

		[SetUp]
		public void Init()
		{
			_linkRepository = MockRepository.GenerateMock<ILinkRepository>();
			_storageService = MockRepository.GenerateMock<IStorage>();

			_deleteLinkCommand = new DeleteLinkCommand(_linkRepository, _storageService);

		}

		[TearDown]
		public void VerifyAllExpectations()
		{
			_linkRepository.VerifyAllExpectations();
			_storageService.VerifyAllExpectations();
		}

		[Test]
		public void Execute_LinkNotFound()
		{
			var argument = Builder<DeleteLinkArgument>.CreateNew()
				.With(x => x.LinkId, Builder<Guid>.CreateNew().Build())
				.Build();

			_linkRepository.Expect(x => x.DeleteLink(Arg<Guid>.Is.Equal(argument.LinkId)))
				.WhenCalled(x => throw new Exception($"Link {x.Arguments[0]} not found."));

			Assert.Throws<Exception>(() => _deleteLinkCommand.Execute(argument));
		}

		[Test]
		public void Execute_Success()
		{
			Guid id = Guid.NewGuid();
			string code = "test";

			Domain domain = Builder<Domain>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.Build();

			var existDbLink = Builder<Repository.Entities.Link>.CreateNew()
				.With(x => x.Id, id)
				.With(x => x.IsActive, true)
				.With(x => x.Code, code)
				.With(x => x.Domain, domain)
				.With(x => x.DomainId, domain.Id)
				.With(x => x.MediaType, MediaType.Music)
				.Build();

			DeleteLinkArgument argument = Builder<DeleteLinkArgument>.CreateNew()
				.With(x => x.LinkId, id)
				.Build();

			_linkRepository.Expect(x => x.DeleteLink(Arg<Guid>.Is.Equal(argument.LinkId))).Return(null)
				.WhenCalled(x =>
				{
					x.ReturnValue = Builder<Repository.Entities.Link>.CreateNew()
					.With(n => n.Id, id)
					.With(n => n.IsActive, false)
					.With(n => n.Code, code)
					.With(n => n.Domain, existDbLink.Domain)
					.With(n => n.DomainId, existDbLink.DomainId)
					.With(n => n.MediaType, existDbLink.MediaType)
					.Build();
				});

			_storageService.Expect(
				x => x.Move(
					Arg<string>.Matches(path => path.Equals($"{domain.Name}/{code}")),
					Arg<string>.Matches(path => path.Equals($"{domain.Name}/{id}"))
				)
			);

			_deleteLinkCommand.Execute(argument);
		}
	}
}