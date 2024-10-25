using DbUp;
using DbUp.Engine;
using System.Reflection;

namespace Laundro.DatabaseMigration;
internal class Migrator
{
    public static DatabaseUpgradeResult Migrate(string connectionString)
    {
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithExecutionTimeout(TimeSpan.FromMinutes(3))
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.StartsWith("Laundro.DatabaseMigration.Scripts"))
            .LogToConsole()
            //.WithTransactionPerScript()
            .Build();
        return upgrader.PerformUpgrade();
    }
}
