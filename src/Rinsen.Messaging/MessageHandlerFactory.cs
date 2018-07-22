using System.Collections.Generic;

namespace Rinsen.Messaging
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly SendGridMessageHandler _sendGrid;

        public MessageHandlerFactory(SendGridMessageHandler sendGrid)
        {
            _sendGrid = sendGrid;
        }

        public IEnumerable<IMessageHandler> CreateHandlers()
        {
            return new [] { _sendGrid };
        }
    }
}
