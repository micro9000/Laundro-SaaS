using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using NodaTime;
using Laundro.Core.NodaTime;

namespace Laundro.Core.Data;
public static class DbContextSoftDeleteExtensions
{
    public static void AddISoftDeleteQueryFilter(this IMutableEntityType entityData)
    {
        var methodToCall = typeof(DbContextSoftDeleteExtensions).GetMethod(nameof(GetIsDeletedFilter), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(entityData.ClrType);

        var filter = methodToCall.Invoke(null, new object[] { });
        entityData.SetQueryFilter((LambdaExpression)filter!);
    }

    public static void ApplySoftDeleteOverride(this ChangeTracker changeTracker, IClockService clock)
    {
        foreach (var entry in changeTracker.Entries())
        {
            if (entry.Entity is ISoftDeletable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues[nameof(ISoftDeletable.IsActive)] = true;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(ISoftDeletable.IsActive)] = false;
                        entry.CurrentValues[nameof(ISoftDeletable.DeActivatedOn)] = clock.Now;
                        break;
                }
            }
        }
    }

    private static LambdaExpression GetIsDeletedFilter<TEntity>() where TEntity : ISoftDeletable // Entity is a base class of all domain classes
    {
        Expression<Func<TEntity, bool>> filter = e => e.IsActive;
        return filter;
    }
}
