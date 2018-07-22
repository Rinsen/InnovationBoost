using System;

namespace Rinsen.Messaging
{
    public class HandledMessage
    {
        public int MessageId { get; set; }

        public MessageHandlerType Type { get; set; }

        public DateTimeOffset? Sent { get; set; }

        public Message Message { get; set; }
    }
}
