using System;
using Repository.Entities;

namespace Repository.Interfaces
{
	public interface IDomainRepository
	{
		Domain GetDomain(Guid id);
	}
}
