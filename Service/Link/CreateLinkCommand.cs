using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;
using Service.Models;
using Service.Models.Link;
using Service.Models.StorageModel.Music;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Link
{
    public class CreateLinkCommand : BaseCommand<Repository.Entities.Link, CreateLinkArgument>, ICommand<LinkModel, CreateLinkArgument>
	{
		private readonly IDomainRepository _domainRepository;
		private readonly IMediaServiceRepository _mediaServiceRepository;

		public CreateLinkCommand(
			ILinkRepository linkRepository,
			IStorage storageService,
			IDomainRepository domainRepository,
			IMediaServiceRepository mediaServiceRepository
		) : base(linkRepository, storageService)
		{
			_domainRepository = domainRepository;
			_mediaServiceRepository = mediaServiceRepository;
		}

		public LinkModel Execute(CreateLinkArgument argument)
		{
			Domain domain = _domainRepository.GetDomain(argument.Link.DomainId);

			if (!string.IsNullOrEmpty(argument.Link.Code))
			{
				if (!LinkHelper.IsValidLinkCode(_storageService, domain.Name, argument.Link.Code))
				{
					throw new ArgumentException($"Shortlink {domain.Name}/{argument.Link.Code} is already in use.");
				}
			}
			else
			{
				argument.Link.Code = LinkHelper.GetUniqueLinkShortCode(_storageService, domain.Name);
			}

			Repository.Entities.Link dbLink = Execute(argument.Link.MediaType, argument, domain);

			return new LinkModel()
			{
				Id = dbLink.Id,
				Code = argument.Link.Code,
				DomainId = domain.Id,
				IsActive = dbLink.IsActive,
				MediaType = dbLink.MediaType,
				Title = dbLink.Title,
				Url = dbLink.Url,
				Artists = dbLink.Artists?.Any() == true
					? dbLink.Artists.Select(x => new ArtistModel()
					{
						Id = x.Id,
						Name = x.Name,
						Label = x.Label
					}).ToList()
					: null
			};
		}

		protected override Repository.Entities.Link ExecuteMusic(CreateLinkArgument argument, params object[] args)
		{
			IEnumerable<Guid> mediaServiceIds = argument.MusicDestinations.SelectMany(
				x => x.Value.Select(
					d => d.MediaServiceId
				)
			).Distinct();

			List<MediaService> mediaServices = _mediaServiceRepository.GetMediaServices().Where(
				x => mediaServiceIds.Contains(x.Id)
			).ToList();

			if (mediaServices.Count <= 0)
            {
				throw new NotSupportedException($"None of media services ids {string.Join(",", mediaServiceIds)} are supported");
			}

			Domain domain = args[0] as Domain;

			string shortLink = LinkHelper.ShortLinkTemplate(domain.Name, argument.Link.Code);
			string generalLinkPath = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

			Repository.Entities.Link dbLink = CreateLink(argument, domain);

			_storageService.Save(generalLinkPath, new StorageModel()
			{
				Id = argument.Link.Id,
				MediaType = argument.Link.MediaType,
				Url = argument.Link.Url,
				Title = argument.Link.Title,
				Destinations = argument.MusicDestinations.ToDictionary(
					md => md.Key,
					md => md.Value.Where(d => mediaServices.Select(m => m.Id).Contains(d.MediaServiceId)).Select(d => new DestinationStorageModel()
					{
						MediaServiceId = d.MediaServiceId,
						TrackingInfo = new TrackingStorageModel()
						{
							MediaServiceName = mediaServices.First(m => m.Id == d.MediaServiceId).Name,

							Artist = d.TrackingInfo?.Artist,
							Album = d.TrackingInfo?.Album,
							SongTitle = d.TrackingInfo?.SongTitle,

							Mobile = d.TrackingInfo?.Mobile,
							Web = d.TrackingInfo?.Web,
						}
					}).ToList())
			});

			return dbLink;
		}

		protected override Repository.Entities.Link ExecuteTicket(CreateLinkArgument argument, params object[] args)
        {
			Domain domain = args[0] as Domain;

			string shortLinkTicket = LinkHelper.ShortLinkTemplate(domain.Name, argument.Link.Code);
			string generalLinkPath = LinkHelper.LinkGeneralFilenameTemplate(shortLinkTicket);

			Repository.Entities.Link dbLink = CreateLink(argument, domain);

			_storageService.Save(generalLinkPath, new Models.StorageModel.Ticket.StorageModel()
			{
				Id = argument.Link.Id,
				MediaType = argument.Link.MediaType,
				Url = argument.Link.Url,
				Title = argument.Link.Title,
				Destinations = argument.TicketDestinations.ToDictionary(
					md => md.Key,
					md => md.Value.Select(d => new Models.StorageModel.Ticket.DestinationStorageModel()
					{
						ShowId = d.ShowId,
						MediaServiceId = d.MediaServiceId,
						Url = d.Url,
						Date = d.Date,
						Location = d.Location,
						Venue = d.Venue
					}).ToList())
			});

			return dbLink;
		}

		private Repository.Entities.Link CreateLink(CreateLinkArgument argument, Domain domain)
		{
			return _linkRepository.CreateLink(new Repository.Entities.Link()
			{
				Code = argument.Link.Code,
				Domain = domain,
				DomainId = domain.Id,
				Id = argument.Link.Id,
				IsActive = argument.Link.IsActive,
				MediaType = argument.Link.MediaType,
				Title = argument.Link.Title,
				Url = argument.Link.Url,
				Artists = argument.Link.Artists?.Any() == true ? argument.Link.Artists.Select(
					x => new Artist()
					{
						Id = x.Id,
						Name = x.Name,
						Label = x.Label
					}
				).ToList() : null
			});
		}
	}

	/// <summary>
	/// Data required for link creation
	/// </summary>
	public class CreateLinkArgument
	{
		public LinkModel Link { get; set; }

		public Dictionary<string, List<Models.Link.Music.DestinationModel>> MusicDestinations { get; set; }

		public Dictionary<string, List<Models.Link.Ticket.DestinationModel>> TicketDestinations { get; set; }
	}
}
