using System;

namespace Rinsen.Messaging
{
    public class PreferedMessageType
    {
        public Guid IdentityId { get; set; }

        public MessageHandlerType Type { get; set; }

        public MessageIdentity MessageIdentity { get; set; }
    }
}