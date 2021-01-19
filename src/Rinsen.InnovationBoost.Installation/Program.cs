using System.Collections.Generic;
using System.Threading.Tasks;
using Rinsen.DatabaseInstaller;

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
            databaseVersions.Add(new SetDatabaseSettingsVersion());
            databaseVersions.Add(new CreateTables());
        }
    }
}
