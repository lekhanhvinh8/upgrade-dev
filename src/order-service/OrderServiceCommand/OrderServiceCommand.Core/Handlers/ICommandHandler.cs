namespace OrderServiceCommand.Core.Commands
{
    public interface ICommandHandler<TCommand, TCommandResult>
    {
        Task<TCommandResult> Handle(TCommand command, CancellationToken cancellation);
    }
}



