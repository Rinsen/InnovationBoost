using System;
using System.Collections.Generic;
using System.Text;
using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.InnovationBoost.Installation.IdentityServer
{
    public class IdentityServerClientTypeTableAdded : DatabaseVersion
    {
        public IdentityServerClientTypeTableAdded()
            : base(3)
        {

        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var identityServerClientTypeTable = dbChangeList.AddNewTable<IdentityServerClientType>();

            identityServerClientTypeTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientTypeTable.AddColumn(m => m.Name, 256).Unique("UX_IdentityServerClientTypes_Name");
            identityServerClientTypeTable.AddColumn(m => m.Created);
            identityServerClientTypeTable.AddColumn(m => m.Updated);

            var identityServerClientTable = dbChangeList.AddNewTableAlteration<IdentityServerClient>();
            identityServerClientTable.AddColumn(m => m.ClientTypeId).Null().ForeignKey<IdentityServerClientType>(m => m.Id);

        }
    }
}
