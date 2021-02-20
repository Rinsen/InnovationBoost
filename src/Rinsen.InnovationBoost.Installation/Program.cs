using System.Collections.Generic;
using System.Threading.Tasks;
using Rinsen.DatabaseInstaller;
using Rinsen.InnovationBoost.Installation.IdentityServer;

namespace Rinsen.InnovationBoost.Installation
{
    class Program
    {
        static Task Main(string[] args)
        {
            return InstallerHost.Start<InstallerStartup>();
        }
    }

    public class InstallerStartup : IInstallerStartup
    {
        public void DatabaseVersionsToInstall(List<DatabaseVersion> databaseVersions)
        {
            databaseVersions.Add(new InitializeDatabase());
            databaseVersions.Add(new CreateTables());
            databaseVersions.Add(new CreateLogTables());
            databaseVersions.Add(new CreateSettingsTable());
            databaseVersions.Add(new CreateAuditLog());
            databaseVersions.Add(new OutbackTableInstallation());

        }
    }
}
