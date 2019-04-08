using System;
using System.Collections.Generic;
using System.Text;
using Rinsen.DatabaseInstaller;

namespace Rinsen.IdentityProvider.Installation
{
    public class IdentityServerTableInstallation : DatabaseVersion
    {
        // https://id4withclients.readthedocs.io/en/latest/id4/ID4Database/DatabaseDiagramID4.html

        public IdentityServerTableInstallation() : base(2)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            throw new NotImplementedException();
        }
    }
}
