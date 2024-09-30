namespace OrderServiceQuery.Core.Commands
{
    public interface ICommandDispatcher
    {
        Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation) where TCommand : BaseCommand;
    }
}

