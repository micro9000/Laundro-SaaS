using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Laundro.Core.Data;
public static class DbContextExtensions
{
    public static SqlServerDbContextOptionsBuilder EnableAzureSqlRetryOnFailure(
        this SqlServerDbContextOptionsBuilder options,
        int maxRetryCount = 6,
        int maxRetryDelaySeconds = 30,
        IEnumerable<int>? additionalErrorNumbers = null)
    {
        //https://learn.microsoft.com/en-us/azure/azure-sql/database/troubleshoot-common-errors-issues?view=azuresql#list-of-transient-fault-error-codes
        var errorNumbersToAdd = new List<int>
        {
            615,
            926,
            4060,
            4221,
            40197,
            40501,
            40613,
            49918,
            49919,
            49920
        };
        errorNumbersToAdd.AddRange(additionalErrorNumbers ?? Array.Empty<int>());

        options.EnableRetryOnFailure(
            maxRetryCount: maxRetryCount,
            maxRetryDelay: TimeSpan.FromSeconds(maxRetryDelaySeconds),
            errorNumbersToAdd: errorNumbersToAdd);

        return options;
    }
}
