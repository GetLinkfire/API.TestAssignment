using System.Linq;
using Repository.Entities;

namespace Repository.Interfaces
{
	public interface IMediaServiceRepository
	{
		IQueryable<MediaService> GetMediaServices();
	}
}
