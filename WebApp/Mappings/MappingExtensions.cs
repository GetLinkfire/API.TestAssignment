using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using WebApp.Models;

namespace WebApp.Mappings
{
	public static class MappingExtensions
	{
		public static List<TDestination> ToDestinationDto<TSource, TDestination>(
			this Dictionary<string, List<TSource>> source)
			where TDestination : DestinationDto
		{
			return source?.SelectMany(keyValuePair =>
				keyValuePair.Value
					.Select(src =>
					{
						var destination = Mapper.Map<TSource, TDestination>(src);
						destination.IsoCode = keyValuePair.Key;
						return destination;
					}).ToList())
				.ToList();
		}
	}
}