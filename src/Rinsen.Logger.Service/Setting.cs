using System;

namespace Rinsen.Logger.Service
{
    public class Setting
    {
        public int Id { get; set; }

        public Guid IdentityId { get; set; }

        public string KeyField { get; set; }

        public string ValueField { get; set; }

        public DateTimeOffset Accessed { get; set; }


    }
}
