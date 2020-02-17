using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.LocalAccounts;
using System.Collections.Generic;

namespace Rinsen.InnovationBoost.Installation
{
    public class CreateTables : DatabaseVersion
    {
        public CreateTables()
        : base(1)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var identitiesTable = dbChangeList.AddNewTable<Identity>("Identities").SetPrimaryKeyNonClustered();
            identitiesTable.AddAutoIncrementColumn(m => m.ClusteredId, primaryKey: false).Unique().Clustered();
            identitiesTable.AddColumn(m => m.IdentityId).PrimaryKey();
            identitiesTable.AddColumn(m => m.Created);
            identitiesTable.AddColumn(m => m.Email, 256).Unique();
            identitiesTable.AddColumn(m => m.EmailConfirmed);
            identitiesTable.AddColumn(m => m.GivenName, 128);
            identitiesTable.AddColumn(m => m.Surname, 128);
            identitiesTable.AddColumn(m => m.PhoneNumber, 128);
            identitiesTable.AddColumn(m => m.PhoneNumberConfirmed);
            identitiesTable.AddColumn(m => m.Updated);

            var identityAttributesTable = dbChangeList.AddNewTable<IdentityAttribute>().SetPrimaryKeyNonClustered();
            identityAttributesTable.AddAutoIncrementColumn(m => m.ClusteredId, primaryKey: false).Unique().Clustered();
            identityAttributesTable.AddColumn(m => m.IdentityId).ForeignKey("Identities", "IdentityId").Unique("UQ_IdentityAndAttribute");
            identityAttributesTable.AddColumn(m => m.Attribute, 256).Unique("UQ_IdentityAndAttribute");

            var localAccountsTable = dbChangeList.AddNewTable<LocalAccount>();
            localAccountsTable.AddAutoIncrementColumn(m => m.Id);
            localAccountsTable.AddColumn(m => m.IdentityId).ForeignKey("Identities", "IdentityId").Unique();
            localAccountsTable.AddColumn(m => m.Created);
            localAccountsTable.AddColumn(m => m.FailedLoginCount);
            localAccountsTable.AddColumn(m => m.IsDisabled);
            localAccountsTable.AddColumn(m => m.IterationCount);
            localAccountsTable.AddColumn(m => m.LoginId, 256).Unique();
            localAccountsTable.AddColumn(m => m.PasswordHash, 16);
            localAccountsTable.AddColumn(m => m.PasswordSalt, 16);
            localAccountsTable.AddColumn(m => m.Updated);

            var sessionsTable = dbChangeList.AddNewTable<Session>().SetPrimaryKeyNonClustered();
            sessionsTable.AddAutoIncrementColumn(m => m.ClusteredId, primaryKey: false).Unique().Clustered();
            sessionsTable.AddColumn(m => m.SessionId, 60).PrimaryKey();
            sessionsTable.AddColumn(m => m.IdentityId).ForeignKey("Identities", "IdentityId");
            sessionsTable.AddColumn(m => m.CorrelationId);
            sessionsTable.AddColumn(m => m.IpAddress, 45);
            sessionsTable.AddColumn(m => m.UserAgent, 200);
            sessionsTable.AddColumn(m => m.Created);
            sessionsTable.AddColumn(m => m.Updated);
            sessionsTable.AddColumn(m => m.Deleted);
            sessionsTable.AddColumn(m => m.Expires);
            sessionsTable.AddColumn(m => m.SerializedTicket);

        }
    }
}


