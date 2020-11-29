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


namespace Service.Links
{
	public class UpdateLinkCommand : ICommand<ExtendedLinkModel, UpdateLink>
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

		public ExtendedLinkModel Execute(UpdateLink request)
		{
			var dbLink = _linkRepository.GetLink(request.Link.Id);

			if (dbLink.MediaType != request.Link.MediaType)
			{
				throw new NotSupportedException("Link media type update is not supported.");
			}

			var domain = _domainRepository.GetDomain(request.Link.DomainId);
			string oldShortLink = null;

			if (dbLink.DomainId != request.Link.DomainId ||
				!dbLink.Code.Equals(request.Link.Code, StringComparison.InvariantCultureIgnoreCase))
			{
				oldShortLink = LinkHelper.ShortLinkTemplate(dbLink.Domain.Name, dbLink.Code);
				if (!LinkHelper.IsValidLinkCode(_storageService, domain.Name, request.Link.Code))
				{
					throw new ArgumentException($"Shortlink {domain.Name}/{request.Link.Code} is already in use.");
				}
			}

			dbLink.Title = request.Link.Title;
			dbLink.Domain = domain;
			dbLink.Code = request.Link.Code;
			dbLink.Url = request.Link.Url;
			dbLink.Artists = GetUpdatedLinkArtists(dbLink.Artists, request.Link.Artists);

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

			var shortLink = LinkHelper.ShortLinkTemplate(domain.Name, request.Link.Code);
			string generalLinkPathToRead = LinkHelper.LinkGeneralFilenameTemplate(oldShortLink ?? shortLink);
			string generalLinkPathToSave = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

			switch (request.Link.MediaType)
			{
				case MediaType.Music:
					SaveMusicLinkToStorage(request, result, generalLinkPathToRead, generalLinkPathToSave);
					break;
				case MediaType.Ticket:
					SaveTicketLinkToStorage(request, result, generalLinkPathToRead, generalLinkPathToSave);
					break;
				default:
					throw new NotSupportedException($"Link type {request.Link.MediaType} is not supported.");
			}

			if (oldShortLink != null)
				_storageService.Delete(oldShortLink);

			return result;

		}

		/// <summary>
		/// Updates the existing artists associated with a link by checking if request artists should be created or updated.
		///	If the GUID already exists in the database the record is updated, otherwise a new record is created.
		/// </summary>
		/// <returns>Updated Artist list</returns>
		private List<Artist> GetUpdatedLinkArtists(ICollection<Artist> existingArtists, List<ArtistModel> artistRequest)
		{
			var updatedArtists = new List<Artist>();

			if (artistRequest == null)
			{
				return updatedArtists;
			}

			foreach (var artist in artistRequest)
			{
				var existingArtist = existingArtists.FirstOrDefault(a => a.Id == artist.Id);

				if (existingArtist != null && 
					existingArtist.Name != artist.Name && 
					existingArtist.Label != artist.Label)
				{
					existingArtist.Name = artist.Name;
					existingArtist.Label = artist.Label;
				}
				else if (existingArtist == null)
				{
					updatedArtists.Add(new Artist(artist.Name, artist.Label));
					continue;
				}

				updatedArtists.Add(existingArtist);
			}

			return updatedArtists;
		}

		private void SaveMusicLinkToStorage(UpdateLink request, ExtendedLinkModel result, string generalLinkPathToRead, string generalLinkPathToSave)
		{
			var uniqMediaServiceIds = request.Link.MusicDestinations
				.SelectMany(x => x.Value.Select(d => d.MediaServiceId))
				.Distinct()
				.ToList();

			var mediaServices = _mediaServiceRepository
				.GetMediaServices()
				.Where(x => uniqMediaServiceIds.Contains(x.Id))
				.ToList();

			var musicStorage = _storageService.Get<StorageModel>(generalLinkPathToRead);

			musicStorage.Title = request.Link.Title;
			musicStorage.Url = request.Link.Url;

			musicStorage.TrackingInfo = request.Link.TrackingInfo == null
				? null
				: new TrackingStorageModel()
				{
					Artist = request.Link.TrackingInfo.Artist,
					Album = request.Link.TrackingInfo.Album,
					SongTitle = request.Link.TrackingInfo.SongTitle,
					Mobile = request.Link.TrackingInfo.Mobile,
					Web = request.Link.TrackingInfo.Web
				};

			musicStorage.Destinations = request.Link.MusicDestinations?.ToDictionary(
				md => md.Key,
				md => md.Value.Where(d => mediaServices.Select(m => m.Id).Contains(d.MediaServiceId)).Select(d =>
					new DestinationStorageModel()
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
					x => x.Value.Select(d => new Models.Link.Music.MusicDestinationModel()
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
		}

		private void SaveTicketLinkToStorage(UpdateLink request, ExtendedLinkModel result, string generalLinkPathToRead, string generalLinkPathToSave)
		{
			var ticketStorage = _storageService.Get<Models.StorageModel.Ticket.StorageModel>(generalLinkPathToRead);

			ticketStorage.Title = request.Link.Title;
			ticketStorage.Url = request.Link.Url;

			// removes keys that are not in argument
			var keysToRemove = new List<string>();
			foreach (var existedIsoCode in ticketStorage.Destinations.Keys)
			{
				if (!request.Link.TicketDestinations.Any(x =>
					x.Key.Equals(existedIsoCode, StringComparison.InvariantCultureIgnoreCase)))
				{
					keysToRemove.Add(existedIsoCode);

				}
			}
			foreach (var key in keysToRemove)
			{
				ticketStorage.Destinations.Remove(key);
			}
			foreach (var ticketDestination in request.Link.TicketDestinations)
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

					var existingExternalIds = ticketStorage.Destinations[existedIsoCode]
						.ToDictionary(x => x.ShowId, x => x.ExternalId);

					ticketStorage.Destinations[existedIsoCode] = ticketDestination.Value.Select(v =>
						new Models.StorageModel.Ticket.DestinationStorageModel()
						{
							ShowId = v.ShowId,
							MediaServiceId = v.MediaServiceId,
							Url = v.Url,
							Date = v.Date,
							Venue = v.Venue,
							Location = v.Location,
							ExternalId = existingExternalIds.ContainsKey(v.ShowId) ? existingExternalIds[v.ShowId] : null
						}).ToList();
				}
			}

			_storageService.Save(generalLinkPathToSave, ticketStorage);

			result.TicketDestinations =
				ticketStorage.Destinations?.ToDictionary(x => x.Key,
					x => x.Value.Select(d => new Models.Link.Ticket.TicketDestinationModel()
					{
						MediaServiceId = d.MediaServiceId,
						Url = d.Url,
						ShowId = d.ShowId,
						Venue = d.Venue,
						Date = d.Date,
						Location = d.Location
					}).ToList());
		}
	}

	public class UpdateLink
	{
		public ExtendedLinkModel Link { get; set; }
	}
}
