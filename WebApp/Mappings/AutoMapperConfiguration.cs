using AutoMapper;

namespace WebApp.Mappings
{
	public static class AutoMapperConfiguration
	{
		public static void Configure()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.AddProfile(new ModelToResponseMappings());
				cfg.AddProfile(new Service.Mappings.Mappings());
			});

		}
	}
}