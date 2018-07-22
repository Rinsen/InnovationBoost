using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.Messaging
{
    public class Message
    {
        public int Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public DateTimeOffset Created { get; set; }

        public MessageIdentity Sender { get; set; }

        public MessageIdentity Receiver { get; set; }

        public List<HandledMessage> HandledMessages { get; set; }

    }
}
