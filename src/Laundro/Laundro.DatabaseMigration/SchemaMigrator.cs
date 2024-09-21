using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using System.Reflection;

namespace Laundro.DatabaseMigration;
internal static class SchemaMigrator
{
    public static DatabaseUpgradeResult Migrate(string connectionString)
    {
        const string dropProgrammabilityObjectsScript = "Laundro.DatabaseMigration.SchemaScripts.DropProgrammabilityObjects.sql";
        const string sharedFolder = "Laundro.DatabaseMigration.SchemaScripts.Shared";
        const string storedProceduresFolder = "Laundro.DatabaseMigration.SchemaScripts.StoredProcedures";
        const string viewsFolder = "Laundro.DatabaseMigration.SchemaScripts.Views";
        const string tvfFolder = "Laundro.DatabaseMigration.SchemaScripts.TVFs";

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .JournalTo(new NullJournal())

            // Order is important - there can be dependecies between objects
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                s => s == dropProgrammabilityObjectsScript,
                new SqlScriptOptions { RunGroupOrder = 0 })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                s => s.StartsWith(sharedFolder),
                new SqlScriptOptions { RunGroupOrder = 1 })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                s => s.StartsWith(tvfFolder),
                new SqlScriptOptions { RunGroupOrder = 2 })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                s => s.StartsWith(viewsFolder),
                new SqlScriptOptions { RunGroupOrder = 2 })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                s => s.StartsWith(storedProceduresFolder),
                new SqlScriptOptions { RunGroupOrder = 3 })
            .LogToConsole()
            .Build();

        return upgrader.PerformUpgrade();
    }
}