﻿using System.Collections.Generic;
using Rinsen.DatabaseInstaller;

namespace Rinsen.InnovationBoost.Installation
{
    public class SetDatabaseSettingsVersion : DatabaseVersion
    {
        public SetDatabaseSettingsVersion()
            :base(1)
        {

        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var databaseSettings = dbChangeList.AddNewDatabaseSettings();

            databaseSettings.CreateLogin("InnovationBoostDebug")
                .WithUser("InnovationBoostDebug")
                .AddRoleMembershipDataReader()
                .AddRoleMembershipDataWriter(); 
        }
    }
}
