using System;
using System.Collections.Generic;

namespace Rinsen.Messaging
{
    public class MessageIdentity
    {
        public Guid IdentityId { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public List<Message> SentMessages { get; set; }

        public List<Message> ReceivedMessages { get; set; }

        public List<PreferedMessageType> PreferedMessageTypes { get; set; }

    }
}
