using System;
using System.Linq;
using AutoMapper;
using Service.Links;
using Service.Models.Link;
using WebApp.Models;

namespace WebApp.Mappings
{
	public class ModelToResponseMappings : Profile
	{
		public ModelToResponseMappings()
		{
			CreateMap<MusicDestinationDto, Service.Models.Link.Music.TrackingModel>()
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore())
				.ForMember(x => x.MediaServiceId, opt => opt.Ignore());

			CreateMap<MusicDestinationDto, Service.Models.Link.Music.MusicDestinationModel>()
				.ForMember(dest => dest.TrackingInfo, opt => opt.MapFrom(x => Mapper.Map<Service.Models.Link.Music.TrackingModel>(x)))
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore())
				.ForMember(x => x.Web, opt => opt.MapFrom(x => x.TrackingInfo.Web))
				.ForMember(x => x.Mobile, opt => opt.MapFrom(x => x.TrackingInfo.Mobile))
				.ForMember(x => x.Artist, opt => opt.MapFrom(x => x.TrackingInfo.Artist))
				.ForMember(x => x.Album, opt => opt.MapFrom(x => x.TrackingInfo.Album))
				.ForMember(x => x.SongTitle, opt => opt.MapFrom(x => x.TrackingInfo.SongTitle));
			
			CreateMap<TicketDestinationDto, Service.Models.Link.Ticket.TicketDestinationModel>()
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore());

			CreateMap<CreateLinkDto, LinkModel>()
				.ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid())) // TODO: This should not be generated here
				.ForMember(x => x.IsActive, opt => opt.MapFrom(x => true))
				.ReverseMap();

			CreateMap<LinkDto, LinkModel>().ReverseMap();

			CreateMap<LinkDto, ExtendedLinkModel>()
				.ForMember(x => x.TrackingInfo, opt => opt.MapFrom(x => Mapper.Map<Service.Models.Link.Music.TrackingModel>(x)))
				.ForMember(x => x.MusicDestinations, opt => opt.Ignore())
				.ForMember(x => x.TicketDestinations, opt => opt.Ignore())
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
				.ForMember(x => x.MusicDestinations, opt => opt.MapFrom(md => md.MusicDestinations.GroupBy(x => x.IsoCode.ToUpper()).ToDictionary(x => x.Key, x => x.Select(Mapper.Map<Service.Models.Link.Music.MusicDestinationModel>).ToList())))
				.ForMember(x => x.TicketDestinations, opt => opt.MapFrom(td => td.TicketDestinations.GroupBy(x => x.IsoCode.ToUpper()).ToDictionary(x => x.Key, x => x.Select(Mapper.Map<Service.Models.Link.Ticket.TicketDestinationModel>).ToList())));

			#endregion
		}
	}
}