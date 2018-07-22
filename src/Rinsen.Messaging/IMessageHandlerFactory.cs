using System.Collections.Generic;

namespace Rinsen.Messaging
{
    public interface IMessageHandlerFactory
    {
        IEnumerable<IMessageHandler> CreateHandlers();
    }
}