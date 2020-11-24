using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
			CreateMap<MusicDestinationDto, Service.Models.Link.Music.DestinationModel>()
				.ForMember(dest => dest.TrackingInfo, opt => opt.MapFrom(x => Mapper.Map<Service.Models.Link.Music.TrackingModel>(x)))
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore())
				.ForMember(x => x.Web, opt => opt.MapFrom(x => x.TrackingInfo.Web))
				.ForMember(x => x.Mobile, opt => opt.MapFrom(x => x.TrackingInfo.Mobile))
				.ForMember(x => x.Artist, opt => opt.MapFrom(x => x.TrackingInfo.Artist))
				.ForMember(x => x.Album, opt => opt.MapFrom(x => x.TrackingInfo.Album))
				.ForMember(x => x.SongTitle, opt => opt.MapFrom(x => x.TrackingInfo.SongTitle));
			
			CreateMap<TicketDestinationDto, Service.Models.Link.Ticket.DestinationModel>()
				.ReverseMap()
				.ForMember(x => x.IsoCode, opt => opt.Ignore());

			CreateMap<CreateLinkDto, LinkModel>()
				.ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid()))
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
		}
	}
}