using System;
using System.Linq;
using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;
using Service.Models;
using Service.Models.Link;

namespace Service.Link
{
	public class GetLinkCommand : BaseCommand<ExtendedLinkModel>, ICommand<ExtendedLinkModel, GetLinkArgument>
	{
		public GetLinkCommand(ILinkRepository linkRepository, IStorage storageService) : base(linkRepository, storageService) { }

		protected override void ExecuteMusic(ExtendedLinkModel argument, params object[] args)
        {
			string generalLinkPath = args[0] as string;

			var musicStorage = _storageService.Get<Models.StorageModel.Music.StorageModel>(generalLinkPath);

			argument.TrackingInfo = musicStorage.TrackingInfo != null ? new Models.Link.Music.TrackingModel
			{
				Artist = musicStorage.TrackingInfo.Artist,
				SongTitle = musicStorage.TrackingInfo.SongTitle,
				Album = musicStorage.TrackingInfo.Album,

				Mobile = musicStorage.TrackingInfo.Mobile,
				Web = musicStorage.TrackingInfo.Web
			} : null;

			argument.MusicDestinations = musicStorage.Destinations?.ToDictionary(
				x => x.Key,
				x => x.Value.Select(
					d => new Models.Link.Music.DestinationModel()
					{
						MediaServiceId = d.MediaServiceId,
						TrackingInfo = d.TrackingInfo != null ? new Models.Link.Music.TrackingModel()
						{
							Artist = d.TrackingInfo.Artist,
							SongTitle = d.TrackingInfo.SongTitle,
							Album = d.TrackingInfo.Album,

							Mobile = d.TrackingInfo.Mobile,
							Web = d.TrackingInfo.Web
						} : null
					}
				).ToList()
			);
		}

		protected override void ExecuteTicket(ExtendedLinkModel argument, params object[] args)
        {
			string generalLinkPath = args[0] as string;

			var ticketStorage = _storageService.Get<Models.StorageModel.Ticket.StorageModel>(generalLinkPath);

			argument.TicketDestinations = ticketStorage.Destinations?.ToDictionary(
				x => x.Key,
				x => x.Value.Select(
					d => new Models.Link.Ticket.DestinationModel()
					{
						MediaServiceId = d.MediaServiceId,
						Url = d.Url,
						ShowId = d.ShowId,
						Venue = d.Venue,
						Date = d.Date,
						Location = d.Location
					}
				).ToList()
			);
		}

		public ExtendedLinkModel Execute(GetLinkArgument argument)
		{
			Repository.Entities.Link dbLink = _linkRepository.GetLink(argument.LinkId);

			string shortLink = LinkHelper.ShortLinkTemplate(dbLink.Domain.Name, dbLink.Code);

			string generalLinkPath = LinkHelper.LinkGeneralFilenameTemplate(shortLink);

			ExtendedLinkModel result = new ExtendedLinkModel()
			{
				Id = dbLink.Id,
				Code = dbLink.Code,
				DomainId = dbLink.Domain.Id,
				IsActive = dbLink.IsActive,
				MediaType = dbLink.MediaType,
				Title = dbLink.Title,
				Url = dbLink.Url,
				Artists = dbLink.Artists?.Any() == true ? dbLink.Artists.Select(
					x => new ArtistModel()
					{
						Id = x.Id,
						Name = x.Name,
						Label = x.Label
					}
				).ToList() : null
			};

			Execute(result.MediaType, result, generalLinkPath);

			return result;
		}
	}

	public class GetLinkArgument
	{
		public Guid LinkId { get; set; }
	}
}
