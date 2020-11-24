using System;
using Repository.Entities;

namespace Repository.Interfaces
{
	public interface ILinkRepository
	{
		/// <summary>
		/// Get active link with domain by id
		/// <exception cref="NotFoundException">when link does not exists in db.</exception>
		/// </summary>
		/// <param name="linkId"></param>
		/// <returns></returns>
		Link GetLink(Guid linkId);

		/// <summary>
		/// Create link
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		Link CreateLink(Link link);

		/// <summary>
		/// Update existing link
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		Link UpdateLink(Link link);
		
		/// <summary>
		/// Delete link logicaly (set IsActive to false)
		/// <exception cref="NotFoundException">when link does not exists in db.</exception>
		/// <exception cref="IllegalArgumentException">when link is not active already.</exception>
		/// </summary>
		/// <param name="linkId"></param>
		Link DeleteLink(Guid linkId);
	}
}
