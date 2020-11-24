namespace Service.Interfaces.Commands
{
	public interface ICommand<TArgument>
	{
		void Execute(TArgument argument);
	}

	public interface ICommand<TPayload, TArgument>
	{
		TPayload Execute(TArgument argument);
	}

}
