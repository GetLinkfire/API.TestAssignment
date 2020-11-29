using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using WebApp.Models;

namespace WebApp.Extensions
{
    public static class DestinationModelExtension
    {
        public static List<MusicDestinationDto> ToDto(this Dictionary<string, List<Service.Models.Link.Music.DestinationModel>> model)
        {
            return model.SelectMany(x =>
                x.Value.Select(d =>
                {
                    MusicDestinationDto dest = Mapper.Map<MusicDestinationDto>(d);
                    
                    dest.IsoCode = x.Key;
                    
                    return dest;
                }).ToList()
            ).ToList();
        }

        public static List<TicketDestinationDto> ToDto(this Dictionary<string, List<Service.Models.Link.Ticket.DestinationModel>> model)
        {
            return model.SelectMany(x =>
                x.Value.Select(d =>
                {
                    TicketDestinationDto dest = Mapper.Map<TicketDestinationDto>(d);
                    
                    dest.IsoCode = x.Key;
                    
                    return dest;
                }).ToList()
            ).ToList();
        }

        public static Dictionary<string, List<Service.Models.Link.Music.DestinationModel>> ToModel(this List<MusicDestinationDto> dto)
        {
            return dto.GroupBy(x => x.IsoCode.ToUpper())
                .ToDictionary(x => x.Key, x => x.Select(Mapper.Map<Service.Models.Link.Music.DestinationModel>).ToList());
        }

        public static Dictionary<string, List<Service.Models.Link.Ticket.DestinationModel>> ToModel(this List<TicketDestinationDto> dto)
        {
            return dto.GroupBy(x => x.IsoCode.ToUpper())
                .ToDictionary(x => x.Key, x => x.Select(Mapper.Map<Service.Models.Link.Ticket.DestinationModel>).ToList());
        }
    }
}