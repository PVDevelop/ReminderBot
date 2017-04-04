using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using PVDevelop.ReminderBot.Microservice.Application;
using PVDevelop.ReminderBot.Microservice.Port.Bus;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Bus
{
    public class InMemoryMessageBus : IMessageBus
    {
        private readonly IEnumerable _commandHandlers;

        public InMemoryMessageBus(IEnumerable commandHandlers)
        {
            if (commandHandlers == null) throw new ArgumentNullException(nameof(commandHandlers));

            _commandHandlers = commandHandlers;
        }

        public void SendMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var commandType = message.Body.GetType();

            var commandHandler = FindCommandHandler(commandType);
            SendCommand(commandHandler, message.Body);
        }

        private object FindCommandHandler(Type commandType)
        {
            return
                _commandHandlers.
                Cast<object>().
                Single(handler => HandlesCommand(handler, commandType));
        }

        private static bool HandlesCommand(object handler, Type commandType)
        {
            var commandHandlerTypeName = typeof(ICommandHandler<>).Name;
            var interfaceType = handler.GetType().GetTypeInfo().GetInterface(commandHandlerTypeName);
            return interfaceType.GenericTypeArguments.Single() == commandType;
        }

        private static void SendCommand(object commandHandler, object command)
        {
            var methodName = nameof(ICommandHandler<object>.Handle);
            var methodInfo = commandHandler.GetType().GetMethod(methodName);
            methodInfo.Invoke(commandHandler, new[] { command });
        }
    }
}
