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
using Service.Models.StorageModel.Ticket;


namespace Service.Link
{
	public class UpdateLinkCommand : ICommand<ExtendedLinkModel, UpdateLinkArgument>
	{
		private readonly IStorage _storageService;
		private readonly IDomainRepository _domainRepository;
		private readonly IMediaServiceRepository _mediaServiceRepository;
		private readonly ILinkRepository _linkRepository;

		public UpdateLinkCommand(
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

		public ExtendedLinkModel Execute(UpdateLinkArgument argument)
		{
			var dbLink = _linkRepository.GetLink(argument.Link.Id);

			if (dbLink.MediaType != argument.Link.MediaType)
			{
				throw new NotSupportedException("Link media type update is not supported.");
			}

			var domain = _domainRepository.GetDomain(argument.Link.DomainId);
			string oldShortLink = null;

			if (dbLink.DomainId != argument.Link.DomainId ||
				!dbLink.Code.Equals(argument.Link.Code, StringComparison.InvariantCultureIgnoreCase))
			{
				oldShortLink = LinkHelper.ShortLinkTemplate(dbLink.Domain.Name, dbLink.Code);
				if (!LinkHelper.IsValidLinkCode(_storageService, domain.Name, argument.Link.Code))
				{
					throw new ArgumentException($"Shortlink {domain.Name}/{argument.Link.Code} is already in use.");
				}
			}

			dbLink.Title = argument.Link.Title;
			dbLink.Domain = domain;
			dbLink.Code = argument.Link.Code;
			dbLink.Url = argument.Link.Url;
			dbLink.Artists = argument.Link.Artists?.Select(x => new Artist()
			{
				Id = x.Id,
				Name = x.Name,
				Label = x.Label
			}).ToList();

			dbLink = _linkRepository.UpdateLink(dbLink);
			var result = new ExtendedLinkModel()
			{
				Id = dbLink.Id,
				Code = dbLink.Code,
				DomainId = domain.Id,
				IsActive = dbLink.IsActive,
				MediaType = dbLink.MediaType,
				Title = dbLink.Title,
				Url = dbLink.Url,
				Artists = dbLink.Artists?.Select(x => new ArtistModel()
				{
					Id = x.Id,
					Name = x.Name,
					Label = x.Label
				}).ToList()
			};

			var shortLink = LinkHelper.ShortLinkTemplate(domain.Name, argument.Link.Code);
			string generalLinkPathToRead = LinkHelper.LinkGeneralFilenameTemplate(oldShortLink ?? shortLink);
			string generalLinkPathToSave = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

			switch (argument.Link.MediaType)
			{
				case MediaType.Music:
					var uniqMediaServiceIds =
						argument.Link.MusicDestinations.SelectMany(x => x.Value.Select(d => d.MediaServiceId)).Distinct().ToList();
					var mediaServices = _mediaServiceRepository.GetMediaServices().Where(x => uniqMediaServiceIds.Contains(x.Id)).ToList();

					var musicStorage = _storageService.Get<Models.StorageModel.Music.StorageModel>(generalLinkPathToRead);

					musicStorage.Title = argument.Link.Title;
					musicStorage.Url = argument.Link.Url;

					musicStorage.TrackingInfo = argument.Link.TrackingInfo == null
						? null
						: new TrackingStorageModel()
						{
							Artist = argument.Link.TrackingInfo.Artist,
							Album = argument.Link.TrackingInfo.Album,
							SongTitle = argument.Link.TrackingInfo.SongTitle,
							Mobile = argument.Link.TrackingInfo.Mobile,
							Web = argument.Link.TrackingInfo.Web
						};

					musicStorage.Destinations = argument.Link.MusicDestinations?.ToDictionary(
						md => md.Key,
						md => md.Value.Where(d => mediaServices.Select(m => m.Id).Contains(d.MediaServiceId)).Select(d =>
							new Models.StorageModel.Music.DestinationStorageModel()
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
							}).ToList());

					_storageService.Save(generalLinkPathToSave, musicStorage);

					result.TrackingInfo = musicStorage.TrackingInfo != null
						? new Models.Link.Music.TrackingModel()
						{
							Artist = musicStorage.TrackingInfo.Artist,
							SongTitle = musicStorage.TrackingInfo.SongTitle,
							Album = musicStorage.TrackingInfo.Album,

							Mobile = musicStorage.TrackingInfo.Mobile,
							Web = musicStorage.TrackingInfo.Web
						}
						: null;

					result.MusicDestinations =
						musicStorage.Destinations?.ToDictionary(x => x.Key,
							x => x.Value.Select(d => new Models.Link.Music.DestinationModel()
							{
								MediaServiceId = d.MediaServiceId,
								TrackingInfo = d.TrackingInfo != null
									? new Models.Link.Music.TrackingModel()
									{
										Artist = d.TrackingInfo.Artist,
										SongTitle = d.TrackingInfo.SongTitle,
										Album = d.TrackingInfo.Album,

										Mobile = d.TrackingInfo.Mobile,
										Web = d.TrackingInfo.Web
									}
									: null
							}).ToList());

					break;
				case MediaType.Ticket:
					var ticketStorage = _storageService.Get<Models.StorageModel.Ticket.StorageModel>(generalLinkPathToRead);

					ticketStorage.Title = argument.Link.Title;
					ticketStorage.Url = argument.Link.Url;

					// removes keys that are not in argument
					var keysToRemove = new List<string>();
					foreach (var existedIsoCode in ticketStorage.Destinations.Keys)
					{
						if (!argument.Link.TicketDestinations.Any(x =>
							x.Key.Equals(existedIsoCode, StringComparison.InvariantCultureIgnoreCase)))
						{
							keysToRemove.Add(existedIsoCode);

						}
					}
					foreach (var key in keysToRemove)
					{
						ticketStorage.Destinations.Remove(key);
					}
					foreach (var ticketDestination in argument.Link.TicketDestinations)
					{
						if (!ticketStorage.Destinations.Any(md =>
							md.Key.Equals(ticketDestination.Key, StringComparison.InvariantCultureIgnoreCase)))
						{
							ticketStorage.Destinations.Add(ticketDestination.Key, ticketDestination.Value.Select(v => new Models.StorageModel.Ticket.DestinationStorageModel()
							{
								ShowId = v.ShowId,
								MediaServiceId = v.MediaServiceId,
								Url = v.Url,
								Date = v.Date,
								Venue = v.Venue,
								Location = v.Location
							}).ToList());
						}
						else
						{
							var existedIsoCode = ticketStorage.Destinations
								.Where(x => x.Key.Equals(ticketDestination.Key, StringComparison.InvariantCultureIgnoreCase))
								.Select(x => x.Key).First();
							var existedExternalIds = ticketStorage.Destinations[existedIsoCode].ToDictionary(x => x.ShowId, x => x.ExternalId);

							ticketStorage.Destinations[existedIsoCode] = ticketDestination.Value.Select(v =>
								new Models.StorageModel.Ticket.DestinationStorageModel()
								{
									ShowId = v.ShowId,
									MediaServiceId = v.MediaServiceId,
									Url = v.Url,
									Date = v.Date,
									Venue = v.Venue,
									Location = v.Location,
									ExternalId = existedExternalIds.ContainsKey(v.ShowId) ? existedExternalIds[v.ShowId] : null
								}).ToList();
						}
					}

					_storageService.Save(generalLinkPathToSave, ticketStorage);

					result.TicketDestinations =
						ticketStorage.Destinations?.ToDictionary(x => x.Key,
							x => x.Value.Select(d => new Models.Link.Ticket.DestinationModel()
							{
								MediaServiceId = d.MediaServiceId,
								Url = d.Url,
								ShowId = d.ShowId,
								Venue = d.Venue,
								Date = d.Date,
								Location = d.Location
							}).ToList());
					break;
				default:
					throw new NotSupportedException($"Link type {argument.Link.MediaType} is not supported.");
			}

			if (oldShortLink != null)
				_storageService.Delete(oldShortLink);

			return result;

		}
	}

	public class UpdateLinkArgument
	{
		public ExtendedLinkModel Link { get; set; }
	}
}
