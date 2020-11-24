using System;
using System.Linq;
using Repository.Entities.Enums;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;
using Service.Models;
using Service.Models.Link;

namespace Service.Link
{
	public class GetLinkCommand : ICommand<ExtendedLinkModel, GetLinkArgument>
	{
		private readonly ILinkRepository _linkRepository;
		private readonly IStorage _storageService;

		public GetLinkCommand(
			IStorage storageService,
			ILinkRepository linkRepository)
		{
			_storageService = storageService;
			_linkRepository = linkRepository;
		}

		public ExtendedLinkModel Execute(GetLinkArgument argument)
		{
			var dbLink = _linkRepository.GetLink(argument.LinkId);

			var shortLink = LinkHelper.ShortLinkTemplate(dbLink.Domain.Name, dbLink.Code);
			string generalLinkPath = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

			var result = new ExtendedLinkModel()
			{
				Id = dbLink.Id,
				Code = dbLink.Code,
				DomainId = dbLink.Domain.Id,
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
					: null,
			};

			switch (dbLink.MediaType)
			{
				case MediaType.Music:
					var musicStorage = _storageService.Get<Models.StorageModel.Music.StorageModel>(generalLinkPath);

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
					var ticketStorage = _storageService.Get<Models.StorageModel.Ticket.StorageModel>(generalLinkPath);

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
					throw new NotSupportedException($"Link type {dbLink.MediaType} is not supported.");
			}

			return result;

		}
	}

	public class GetLinkArgument
	{
		public Guid LinkId { get; set; }
	}
}
