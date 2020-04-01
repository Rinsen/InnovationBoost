using System;
using System.Collections.Generic;
using System.Text;
using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.SqlTypes;
using Rinsen.Logger.Service;

namespace Rinsen.InnovationBoost.Installation
{
    public class CreateLogTables : DatabaseVersion
    {
        public CreateLogTables() 
            : base(3)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var logSource = dbChangeList.AddNewTable<LogSource>();
            logSource.AddAutoIncrementColumn(m => m.Id);
            logSource.AddColumn(m => m.Name, 100).Unique("UQ_LogSources_Name").NotNull();

            var logEnvironmentTable = dbChangeList.AddNewTable<LogEnvironment>();
            logEnvironmentTable.AddAutoIncrementColumn(m => m.Id);
            logEnvironmentTable.AddColumn(m => m.Name, 100).Unique("UQ_LogEnvironments_Name").NotNull();

            var logApplicationTable = dbChangeList.AddNewTable<LogApplication>();
            logApplicationTable.AddAutoIncrementColumn(m => m.Id);
            logApplicationTable.AddColumn(m => m.ApplicationId, 100).Unique("UQ_LogApplications_Name").NotNull();
            logApplicationTable.AddColumn(m => m.DisplayName, 100).NotNull();

            var logsTable = dbChangeList.AddNewTable<Log>();
            logsTable.AddAutoIncrementColumn(m => m.Id);
            logsTable.AddColumn(m => m.ApplicationId).NotNull().ForeignKey<LogApplication>(m => m.Id);
            logsTable.AddColumn(m => m.EnvironmentId).NotNull().ForeignKey<LogEnvironment>(m => m.Id);
            logsTable.AddColumn(m => m.SourceId).NotNull().ForeignKey<LogSource>(m => m.Id);
            logsTable.AddColumn(m => m.RequestId, 100).NotNull();
            logsTable.AddIntColumn("LogLevel").NotNull();
            logsTable.AddColumn(m => m.MessageFormat, int.MaxValue).NotNull();
            logsTable.AddColumn("LogProperties", new NVarChar()).NotNull();
            logsTable.AddColumn(m => m.Timestamp).NotNull();
        }
    }
}
