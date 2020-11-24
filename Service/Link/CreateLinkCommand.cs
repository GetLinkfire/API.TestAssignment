using System;
using System.Collections.Generic;
using System.Linq;
using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;
using Service.Models;
using Service.Models.Link;
using Service.Models.StorageModel.Music;

namespace Service.Link
{
	public class CreateLinkCommand : ICommand<LinkModel, CreateLinkArgument>
	{
		private readonly IStorage _storageService;
		private readonly IDomainRepository _domainRepository;
		private readonly IMediaServiceRepository _mediaServiceRepository;
		private readonly ILinkRepository _linkRepository;

		public CreateLinkCommand(
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


		public LinkModel Execute(CreateLinkArgument argument)
		{
			var domain = _domainRepository.GetDomain(argument.Link.DomainId);

			var code = argument.Link.Code;
			if (!String.IsNullOrEmpty(code))
			{
				if (!LinkHelper.IsValidLinkCode(_storageService, domain.Name, argument.Link.Code))
				{
					throw new ArgumentException($"Shortlink {domain.Name}/{argument.Link.Code} is already in use.");
				}
			}
			else
			{
				code = LinkHelper.GetUniqueLinkShortCode(_storageService, domain.Name);
			}

			Repository.Entities.Link dbLink;

			switch (argument.Link.MediaType)
			{
				case MediaType.Music:
					var uniqMediaServiceIds =
						argument.MusicDestinations.SelectMany(x => x.Value.Select(d => d.MediaServiceId)).Distinct().ToList();
					var mediaServices = _mediaServiceRepository.GetMediaServices().Where(x => uniqMediaServiceIds.Contains(x.Id)).ToList();

					var shortLink = LinkHelper.ShortLinkTemplate(domain.Name, code);
					string generalLinkPath = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

					dbLink = _linkRepository.CreateLink(new Repository.Entities.Link()
					{
						Code = code,
						Domain = domain,
						DomainId = domain.Id,
						Id = argument.Link.Id,
						IsActive = argument.Link.IsActive,
						MediaType = argument.Link.MediaType,
						Title = argument.Link.Title,
						Url = argument.Link.Url,
						Artists = argument.Link.Artists?.Any() == true
							? argument.Link.Artists.Select(x => new Artist()
							{
								Id = x.Id,
								Name = x.Name,
								Label = x.Label
							}).ToList()
							: null
					});

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

					break;
				case MediaType.Ticket:

					var shortLinkTicket = LinkHelper.ShortLinkTemplate(domain.Name, code);
					string generalLinkPathTicket = LinkHelper.LinkGeneralFilenameTemplate(shortLinkTicket);

					dbLink = _linkRepository.CreateLink(new Repository.Entities.Link()
					{
						Id = argument.Link.Id,
						Code = code,
						Domain = domain,
						DomainId = domain.Id,
						IsActive = true,
						MediaType = argument.Link.MediaType,
						Title = argument.Link.Title,
						Url = argument.Link.Url,
						Artists = argument.Link.Artists?.Any() == true
							? argument.Link.Artists.Select(x => new Artist()
							{
								Id = x.Id,
								Name = x.Name,
								Label = x.Label
							}).ToList()
							: null
					});

					_storageService.Save(generalLinkPathTicket, new Models.StorageModel.Ticket.StorageModel()
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
					break;
				default:
					throw new NotSupportedException($"Link type {argument.Link.MediaType} is not supported.");
			}

			return new LinkModel()
			{
				Id = dbLink.Id,
				Code = code,
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
