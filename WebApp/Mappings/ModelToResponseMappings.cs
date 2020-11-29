using System.Linq;
using AutoMapper;
using Service.Links;
using Service.Models.Link;
using Service.Models.Link.Music;
using Service.Models.Link.Ticket;
using WebApp.Models;

namespace WebApp.Mappings
{
	public class ModelToResponseMappings : Profile
	{
		public ModelToResponseMappings()
		{
			CreateMap<MusicDestinationDto, TrackingModel>()
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore())
				.ForMember(x => x.MediaServiceId, opt => opt.Ignore());

			CreateMap<MusicDestinationDto, MusicDestinationModel>()
				.ForMember(dest => dest.TrackingInfo, opt => opt.MapFrom(x => Mapper.Map<TrackingModel>(x)))
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore())
				.ForMember(x => x.Web, opt => opt.MapFrom(x => x.TrackingInfo.Web))
				.ForMember(x => x.Mobile, opt => opt.MapFrom(x => x.TrackingInfo.Mobile))
				.ForMember(x => x.Artist, opt => opt.MapFrom(x => x.TrackingInfo.Artist))
				.ForMember(x => x.Album, opt => opt.MapFrom(x => x.TrackingInfo.Album))
				.ForMember(x => x.SongTitle, opt => opt.MapFrom(x => x.TrackingInfo.SongTitle));
			
			CreateMap<TicketDestinationDto, TicketDestinationModel>()
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore());

			CreateMap<CreateLinkDto, LinkModel>() 
				.ForMember(x => x.IsActive, opt => opt.MapFrom(x => true))
				.ReverseMap();

			CreateMap<LinkDto, LinkModel>()
				.ReverseMap();

			CreateMap<LinkDto, ExtendedLinkModel>()
				.ForMember(x => x.TrackingInfo, opt => opt.MapFrom(x => Mapper.Map<TrackingModel>(x)))
				.ForMember(x => x.MusicDestinations, opt => opt.MapFrom(md => md.MusicDestinations.GroupBy(x => x.IsoCode.ToUpper()).ToDictionary(x => x.Key, x => x.Select(Mapper.Map<MusicDestinationModel>).ToList())))
				.ForMember(x => x.TicketDestinations, opt => opt.MapFrom(td => td.TicketDestinations.GroupBy(x => x.IsoCode.ToUpper()).ToDictionary(x => x.Key, x => x.Select(Mapper.Map<MusicDestinationModel>).ToList())))
				.ReverseMap()
				.ForMember(x => x.Web, opt => opt.MapFrom(x => x.TrackingInfo.Web))
				.ForMember(x => x.Mobile, opt => opt.MapFrom(x => x.TrackingInfo.Mobile))
				.ForMember(x => x.Artist, opt => opt.MapFrom(x => x.TrackingInfo.Artist))
				.ForMember(x => x.Album, opt => opt.MapFrom(x => x.TrackingInfo.Album))
				.ForMember(x => x.SongTitle, opt => opt.MapFrom(x => x.TrackingInfo.SongTitle))
				.ForMember(x => x.MusicDestinations, opt => opt.Ignore())
				.ForMember(x => x.TicketDestinations, opt => opt.Ignore());

			#region Create Link
			CreateMap<CreateLinkDto, CreateLink>()
				.ForMember(x => x.Link, opt => opt.MapFrom(x => Mapper.Map<LinkModel>(x)))
				.ForMember(x => x.MusicDestinations, opt => opt.MapFrom(md => md.MusicDestinations.GroupBy(x => x.IsoCode.ToUpper()).ToDictionary(x => x.Key, x => x.Select(Mapper.Map<MusicDestinationModel>).ToList())))
				.ForMember(x => x.TicketDestinations, opt => opt.MapFrom(td => td.TicketDestinations.GroupBy(x => x.IsoCode.ToUpper()).ToDictionary(x => x.Key, x => x.Select(Mapper.Map<TicketDestinationModel>).ToList())));

			#endregion

			#region Update Link
			CreateMap<LinkDto, UpdateLink>()
				.ForMember(x => x.Link, opt => opt.MapFrom(x => Mapper.Map<ExtendedLinkModel>(x)));

			#endregion
		}

	}
}