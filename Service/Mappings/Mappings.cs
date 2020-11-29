using AutoMapper;
using Repository.Entities;
using Service.Models.Link;

namespace Service.Mappings
{
	public class Mappings : Profile
	{
		public Mappings()
		{
			AllowNullCollections = true;
			CreateMap<Link, LinkModel>();
		}
	}
}
