using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;

namespace Repository
{
    public class LinkRepository : ILinkRepository
	{
		private readonly Context _context;

		public LinkRepository(Context context)
		{
			_context = context;
		}

		public Link GetLink(Guid linkId)
		{
			Link link = _context.Links.Include(x => x.Domain).FirstOrDefault(x => x.Id == linkId && x.IsActive);
			
			if (link == null)
			{
				throw new ObjectNotFoundException($"Link {linkId} not found.");
			}

			return link;
		}

		public Link CreateLink(Link link)
		{
			_context.Domains.Attach(link.Domain);

			FetchArtists(link);

			link.IsActive = true;
			
			_context.Links.Add(link);
			_context.SaveChanges();
			
			return link;
		}

		public Link UpdateLink(Link link)
		{
			FetchArtists(link);

			return UpdateLink(link, false);
		}

		private Link UpdateLink(Link link, bool isDelete)
		{
			var entry = _context.Entry(link);

			// make sure that next fields will be never modified on update
			entry.Property(x => x.MediaType).IsModified = false;

			if (!isDelete)
			{
				entry.Property(x => x.IsActive).IsModified = false;
			}

			_context.Domains.Attach(link.Domain);

			entry.State = EntityState.Modified;
			_context.SaveChanges();

			return link;
		}

		public Link DeleteLink(Guid linkId)
		{
			Link link = GetLink(linkId);
			
			link.IsActive = false;

			return UpdateLink(link, true);
		}

		private void FetchArtists(Link link)
        {
			if (link.Artists?.Any() == true)
			{
				List<Guid> linkArtistIds = link.Artists.Select(x => x.Id).ToList();
				var artists = _context.Artists.Where(x => linkArtistIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x);

				foreach (Artist artist in link.Artists)
				{
					if (!artists.ContainsKey(artist.Id))
					{
						artists.Add(artist.Id, artist);
					}
				}

				link.Artists = artists.Select(x => x.Value).ToList();
			}
		}
	}
}