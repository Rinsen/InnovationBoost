using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public interface IExternalSessionStorage
    {
        Task Create(ExternalSession externalSession);
    }
}
