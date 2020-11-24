using System;
using Service.Interfaces.Commands;

namespace Service.Link
{
	public class DeleteLinkCommand: ICommand<DeleteLinkArgument>
	{
		public void Execute(DeleteLinkArgument argument)
		{
			// TODO: implement delete link command
			throw new NotImplementedException();
		}
	}

	public class DeleteLinkArgument
	{
		public Guid LinkId { get; set; }
	}
}
