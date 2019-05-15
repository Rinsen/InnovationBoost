using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public interface IObjectWithState
    {
        ObjectState State { get; set; }
    }

    public enum ObjectState
    {
        Unchanged = 0,
        Added = 1,
        Modified = 2,
        Deleted = 3,
        AddedThenDeleted = 4
    }
}
