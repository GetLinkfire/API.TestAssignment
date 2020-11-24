using System;
using System.Linq;
using Repository.Entities;

namespace Repository.Interfaces
{
	public interface IDomainRepository
	{
		Domain GetDomain(Guid id);
	}
}
