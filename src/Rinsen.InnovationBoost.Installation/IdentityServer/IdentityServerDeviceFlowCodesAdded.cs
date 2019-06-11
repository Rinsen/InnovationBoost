using System;
using System.Collections.Generic;
using System.Text;
using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.InnovationBoost.Installation.IdentityServer
{
    public class IdentityServerDeviceFlowCodesAdded :  DatabaseVersion
    {
        public IdentityServerDeviceFlowCodesAdded()
            : base(2)
        {

        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var identityServerDeviceFlowCode = dbChangeList.AddNewTable<IdentityServerDeviceFlowCode>();

            identityServerDeviceFlowCode.AddAutoIncrementColumn(m => m.Id);
            identityServerDeviceFlowCode.AddColumn(m => m.DeviceCode, length: 200).Unique("UX_IdentityServerDeviceFlowCodes_DeviceCode");
            identityServerDeviceFlowCode.AddColumn(m => m.UserCode, length: 200).Unique("UX_IdentityServerDeviceFlowCodes_UserCode"); ;
            identityServerDeviceFlowCode.AddColumn(m => m.ClientId, length: 200).ForeignKey<IdentityServerClient>(m => m.ClientId);
            identityServerDeviceFlowCode.AddColumn(m => m.CreationTime);
            identityServerDeviceFlowCode.AddColumn(m => m.Expiration);
            identityServerDeviceFlowCode.AddColumn(m => m.IdentityId).ForeignKey("Identities", "IdentityId");
            identityServerDeviceFlowCode.AddColumn(m => m.SerializedDeviceFlowCode);

        }
    }
}
