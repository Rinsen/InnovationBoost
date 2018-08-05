﻿using Rinsen.DatabaseInstaller;
using Rinsen.Logger.Service;
using System.Collections.Generic;

namespace Rinsen.InnovationBoost.Installation
{
    public class CreateSettingsTable : DatabaseVersion
    {
        public CreateSettingsTable()
            : base(2)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var settingTable = dbChangeList.AddNewTable<Setting>();

            settingTable.AddAutoIncrementColumn(m => m.Id);
            settingTable.AddColumn(m => m.IdentityId).Unique("UX_Settings_IdentityId_KeyField");
            settingTable.AddColumn(m => m.KeyField, length: 100).Unique("UX_Settings_IdentityId_KeyField");
            settingTable.AddColumn(m => m.ValueField, length: 4000);
            settingTable.AddColumn(m => m.Accessed);
        }
    }
}