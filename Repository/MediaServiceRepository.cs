using System.Linq;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository
{
	public class MediaServiceRepository : IMediaServiceRepository
	{
		private readonly Context _context;

		public MediaServiceRepository(Context context)
		{
			_context = context;
		}

		public IQueryable<MediaService> GetMediaServices()
		{
			return _context.MediaServices;
		}
	}
}
