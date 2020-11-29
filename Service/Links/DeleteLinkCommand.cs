using System;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;

namespace Service.Links
{
	public class DeleteLinkCommand: ICommand<DeleteLink>
	{
		private readonly IStorage _storageService;
		private readonly IDomainRepository _domainRepository;
		private readonly IMediaServiceRepository _mediaServiceRepository;
		private readonly ILinkRepository _linkRepository;
			
		public DeleteLinkCommand(
			IStorage storageService,
			IDomainRepository domainRepository,
			IMediaServiceRepository mediaServiceRepository,
			ILinkRepository linkRepository)
		{
			_storageService = storageService;
			_domainRepository = domainRepository;
			_mediaServiceRepository = mediaServiceRepository;
			_linkRepository = linkRepository;
		}
		public void Execute(DeleteLink request)
		{
			var deletedLink = _linkRepository.DeleteLink(request.LinkId);
			var domain = _domainRepository.GetDomain(deletedLink.DomainId);

			var shortLink = LinkHelper.ShortLinkTemplate(domain.Name, deletedLink.Code);
			var deletedFilePath = LinkHelper.ShortLinkTemplate(domain.Name, deletedLink.Id.ToString());
			_storageService.RenameDirectory(shortLink, deletedFilePath);
		}
	}

	public class DeleteLink
	{
		public Guid LinkId { get; set; }
	}
}
