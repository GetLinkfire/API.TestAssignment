using System;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;

namespace Service.Link
{
	public class DeleteLinkCommand : ICommand<DeleteLinkArgument>
	{
		private readonly ILinkRepository _linkRepository;
		private readonly IStorage _storageService;

		public DeleteLinkCommand(ILinkRepository linkRepository, IStorage storageService)
		{
			_linkRepository = linkRepository;
			_storageService = storageService;
		}

		public void Execute(DeleteLinkArgument argument)
		{
			Repository.Entities.Link dbLink = _linkRepository.DeleteLink(argument.LinkId);

			string oldShortLink = LinkHelper.ShortLinkTemplate(dbLink.Domain.Name, dbLink.Code),
			deletedLink = LinkHelper.DeletedLinkTemplate(dbLink.Domain.Name, dbLink.Id);

			_storageService.Move(oldShortLink, deletedLink);
		}
    }

	public class DeleteLinkArgument
	{
		public Guid LinkId { get; set; }
	}
}
