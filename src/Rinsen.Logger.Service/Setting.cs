using System;

namespace Rinsen.Logger.Service
{
    public class Setting
    {
        public int Id { get; set; }

        public Guid IdentityId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset Accessed { get; set; }


    }
}
