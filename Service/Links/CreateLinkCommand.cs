using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;
using Service.Models.Link;
using Service.Models.StorageModel.Music;

namespace Service.Links
{
	public class CreateLinkCommand : ICommand<LinkModel, CreateLink>
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


		public LinkModel Execute(CreateLink request)
		{
			var domain = _domainRepository.GetDomain(request.Link.DomainId);

			var code = request.Link.Code;

			if (!string.IsNullOrEmpty(code))
			{
				if (!LinkHelper.IsValidLinkCode(_storageService, domain.Name, request.Link.Code))
				{
					throw new ArgumentException($"Shortlink {domain.Name}/{request.Link.Code} is already in use.");
				}
			}
			else
			{
				code = LinkHelper.GetUniqueLinkShortCode(_storageService, domain.Name);
			}

			Link dbLink;

			switch (request.Link.MediaType)
			{
				case MediaType.Music:
					var uniqMediaServiceIds =
						request.MusicDestinations
						.SelectMany(x =>
							x.Value.Select(d => d.MediaServiceId))
						.Distinct()
						.ToList();

					var mediaServices = _mediaServiceRepository
						.GetMediaServices()
						.Where(x =>
							uniqMediaServiceIds.Contains(x.Id))
						.ToList();

					var shortLink = LinkHelper.ShortLinkTemplate(domain.Name, code);
					string generalLinkPath = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

					dbLink = CreateDbLink(request, domain, code);

					SaveMusicLinkToStorage(request, mediaServices, generalLinkPath);

					break;
				case MediaType.Ticket:
					var shortLinkTicket = LinkHelper.ShortLinkTemplate(domain.Name, code);
					string generalLinkPathTicket = LinkHelper.LinkGeneralFilenameTemplate(shortLinkTicket);

					dbLink = CreateDbLink(request, domain, code);
					SaveTicketLinkToStorage(request, generalLinkPathTicket);
					break;
				default:
					throw new NotSupportedException($"Link type {request.Link.MediaType} is not supported.");
			}

			return Mapper.Map<LinkModel>(dbLink);
		}

		private void SaveTicketLinkToStorage(CreateLink request, string generalLinkPathTicket)
		{
			_storageService.Save(generalLinkPathTicket, new Models.StorageModel.Ticket.StorageModel()
			{
				Id = request.Link.Id,
				MediaType = request.Link.MediaType,
				Url = request.Link.Url,
				Title = request.Link.Title,
				Destinations = request.TicketDestinations.ToDictionary(
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
		}

		private void SaveMusicLinkToStorage(CreateLink request, List<MediaService> mediaServices, string generalLinkPath)
		{
			_storageService.Save(generalLinkPath, new StorageModel()
			{
				Id = request.Link.Id,
				MediaType = request.Link.MediaType,
				Url = request.Link.Url,
				Title = request.Link.Title,
				Destinations = request.MusicDestinations.ToDictionary(
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
		}

		private Link CreateDbLink(CreateLink request, Domain domain, string code)
		{
			var artists = request.Link.Artists?
				.Select(artist =>
					new Artist(artist.Name, artist.Label))
				.ToList();

			var newLink = new Link(
				request.Link.Title,
				request.Link.MediaType,
				code,
				domain,
				request.Link.Url,
				artists);

			return _linkRepository.CreateLink(newLink);
		}
	}

	/// <summary>
	/// Data required for link creation
	/// </summary>
	public class CreateLink
	{
		public LinkModel Link { get; set; }

		public Dictionary<string, List<Models.Link.Music.MusicDestinationModel>> MusicDestinations { get; set; }

		public Dictionary<string, List<Models.Link.Ticket.TicketDestinationModel>> TicketDestinations { get; set; }
	}
}
