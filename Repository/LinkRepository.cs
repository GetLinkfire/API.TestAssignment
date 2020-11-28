using System;
using System.Linq;
using Repository.Entities;
using Repository.Interfaces;
using System.Data.Entity;

namespace Repository
{
	public class LinkRepository : Repository<Link>, ILinkRepository
	{
		public LinkRepository(Context dbContext) : base(dbContext) { }

		public Link GetLink(Guid linkId)
		{
			var link = _dbContext.Links
				.Include(x => x.Domain)
				.FirstOrDefault(x =>
					x.Id == linkId &&
					x.IsActive);

			if (link == null)
			{
				throw new Exception($"Link {linkId} not found.");
			}

			return link;
		}

		public Link CreateLink(Link link)
		{

			_dbContext.Domains.Attach(link.Domain);

			if (link.Artists?.Any() == true)
			{
				var linkArtistIds = link.Artists.Select(x => x.Id).ToList();
				var artists = _dbContext.Artists
					.Where(x => linkArtistIds.Contains(x.Id))
					.ToDictionary(x => x.Id, x => x);

				foreach (var artist in link.Artists)
				{
					if (!artists.ContainsKey(artist.Id))
						artists.Add(artist.Id, artist);
				}

				link.Artists = artists.Select(x => x.Value).ToList();
			}

			link.IsActive = true;
			_dbContext.Links.Add(link);
			_dbContext.SaveChanges();
			return link;
		}

		public Link UpdateLink(Link link)
		{
			var entry = _dbContext.Entry(link);

			// make sure that next fields will be never modified on update
			entry.Property(x => x.MediaType).IsModified = false;
			entry.Property(x => x.IsActive).IsModified = false;

			_dbContext.Domains.Attach(link.Domain);

			// TODO: implement DB link update
			throw new NotImplementedException();
		}

		public Link DeleteLink(Guid linkId)
		{
			var link = _dbContext.Links.Include(x => x.Domain).FirstOrDefault(x => x.Id == linkId);
			if (link == null)
			{
				throw new Exception($"Link {linkId} not found.");
			}
			link.IsActive = false;
			_dbContext.SaveChanges();
			return link;
		}
	}
}
