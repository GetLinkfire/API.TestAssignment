using System;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository
{
	public class DomainRepository : IDomainRepository
	{
		private readonly Context _context;

		public DomainRepository(Context context)
		{
			_context = context;
		}

		public Domain GetDomain(Guid id)
		{
			var domain = _context.Domains.Find(id);
			if (domain == null)
			{
				throw new Exception($"Domain {id} not found.");
			}

			return domain;
		}
	}
}
