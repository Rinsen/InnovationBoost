﻿using System.Collections.Generic;
using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider.AuditLogging;

namespace Rinsen.InnovationBoost.Installation
{
    public class CreateAuditLog : DatabaseVersion
    {
        public CreateAuditLog()
            : base(5)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var auditLogItems = dbChangeList.AddNewTable<AuditLogItem>();

            auditLogItems.AddAutoIncrementColumn(m => m.Id);
            auditLogItems.AddColumn(m => m.EventType, 256);
            auditLogItems.AddColumn(m => m.Details, 4000);
            auditLogItems.AddColumn(m => m.IpAddress, 45);
            auditLogItems.AddColumn(m => m.Timestamp);
        }
    }
}
