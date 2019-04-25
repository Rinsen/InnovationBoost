using System;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerPersistedGrant : IObjectWithState
    {
        public int Id { get; set; }

        public string Key { get; set; }
        
        public string Type { get; set; }
        
        public string SubjectId { get; set; }
        
        public string ClientId { get; set; }
        
        public DateTimeOffset CreationTime { get; set; }
        
        public DateTimeOffset? Expiration { get; set; }
        
        public string Data { get; set; }

        public ObjectState State { get; set; }
    }
}
