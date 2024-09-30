using Microsoft.Extensions.DependencyInjection;
using OrderServiceCommand.Core.Commands;

namespace OrderServiceCommand.Infrastructure.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation) where TCommand : BaseCommand
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
            return handler.Handle(command, cancellation);
        }
    }
}


