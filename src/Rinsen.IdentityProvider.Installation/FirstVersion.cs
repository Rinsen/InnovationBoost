using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.ExternalApplications;
using Rinsen.IdentityProvider.LocalAccounts;
using System.Collections.Generic;

namespace Rinsen.IdentityProvider.Installation
{
    public class FirstVersion : DatabaseVersion
    {
        public FirstVersion()
            :base(1)
        {
        }

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
            sessionsTable.AddColumn(m => m.LastAccess);
            sessionsTable.AddColumn(m => m.Expires);
            sessionsTable.AddColumn(m => m.SerializedTicket);

            var externalApplicationTable = dbChangeList.AddNewTable<ExternalApplication>().SetPrimaryKeyNonClustered();
            externalApplicationTable.AddAutoIncrementColumn(m => m.Id, primaryKey: false).Unique().Clustered();
            externalApplicationTable.AddColumn(m => m.ExternalApplicationId).PrimaryKey();
            externalApplicationTable.AddColumn(m => m.Active);
            externalApplicationTable.AddColumn(m => m.ActiveUntil);
            externalApplicationTable.AddColumn(m => m.ApplicationKey, 256);
            externalApplicationTable.AddColumn(m => m.Created);
            externalApplicationTable.AddColumn(m => m.Name, 256).Unique();

            var externalApplicationHostNameTable = dbChangeList.AddNewTable<ExternalApplicationHostName>().SetPrimaryKeyNonClustered();
            externalApplicationHostNameTable.AddColumn(m => m.ExternalApplicationId).ForeignKey<ExternalApplication>(m => m.ExternalApplicationId);
            externalApplicationHostNameTable.AddColumn(m => m.Hostname, 512).PrimaryKey();
            externalApplicationHostNameTable.AddColumn(m => m.Active);
            externalApplicationHostNameTable.AddColumn(m => m.ActiveUntil);
            externalApplicationHostNameTable.AddColumn(m => m.Created);

            var tokenTable = dbChangeList.AddNewTable<Token>().SetPrimaryKeyNonClustered();
            tokenTable.AddAutoIncrementColumn(m => m.ClusteredId, primaryKey: false).Unique().Clustered();
            tokenTable.AddColumn(m => m.ExternalApplicationId).ForeignKey<ExternalApplication>(m => m.ExternalApplicationId);
            tokenTable.AddColumn(m => m.Created);
            tokenTable.AddColumn(m => m.CorrelationId);
            tokenTable.AddColumn(m => m.Expiration);
            tokenTable.AddColumn(m => m.IdentityId);
            tokenTable.AddColumn(m => m.TokenId, 50).PrimaryKey();

            var externalSessionsTable = dbChangeList.AddNewTable<ExternalSession>();
            externalSessionsTable.AddAutoIncrementColumn(m => m.Id);
            externalSessionsTable.AddColumn(m => m.IdentityId).ForeignKey("Identities", "IdentityId");
            externalSessionsTable.AddColumn(m => m.Created);
            externalSessionsTable.AddColumn(m => m.CorrelationId);
            externalSessionsTable.AddColumn(m => m.ExternalApplicationId).ForeignKey<ExternalApplication>(m => m.ExternalApplicationId);
        }
    }
}