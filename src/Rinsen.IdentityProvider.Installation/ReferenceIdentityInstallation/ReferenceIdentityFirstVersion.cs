using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider.Core;
using System.Collections.Generic;

namespace Rinsen.IdentityProvider.Installation.ReferenceIdentityInstallation
{
    public class ReferenceIdentityFirstVersion : DatabaseVersion
    {
        public ReferenceIdentityFirstVersion()
            : base(1)
        {
            
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var referenceIdentityTable =  dbChangeList.AddNewTable<ReferenceIdentity>("ReferenceIdentities");

            referenceIdentityTable.AddAutoIncrementColumn(m => m.ClusteredId, primaryKey: false);
            referenceIdentityTable.AddColumn(m => m.IdentityId).PrimaryKey();
            referenceIdentityTable.AddColumn(m => m.Created);

        }
    }
}
