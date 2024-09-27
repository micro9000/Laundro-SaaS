using Dapper;
using Laundro.MicrosoftEntraId.AuthExtension.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface IClaimsRepository
{
    Task<UserInfo?> GetUserInfo(string email);
    Task<int?> GetTenantId(int userId);
    Task<IEnumerable<StoreInfo>> GetStoresByManagerId(int userId);
    Task<IEnumerable<StoreInfo>> GetStoresByStaffId(int userId);
}

public class ClaimsRepository : IClaimsRepository
{
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public ClaimsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = _configuration.GetConnectionString("LaundroConnectionString");
        if (connectionString.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        _connectionString = connectionString;
    }

    public async Task<UserInfo?> GetUserInfo(string email)
    {
        UserInfo? userInfo = null;

        using (DbConnection connection = new SqlConnection(_connectionString))
        {
            var query = @"SELECT TOP 1 U.Id As UserId, R.SystemKey As Role
                            FROM Users U
                            JOIN Roles R ON R.Id=U.RoleId
                            WHERE U.IsActive=1 AND R.IsActive=1 AND U.Email=@Email";

            userInfo = await connection.QueryFirstOrDefaultAsync<UserInfo>(query, new
            {
                Email = email
            });
        }

        return userInfo;
    }

    public async Task<int?> GetTenantId(int userId)
    {
        int? tenantId = null;

        using (DbConnection connection = new SqlConnection(_connectionString))
        {
            var query = @"SELECT TOP 1 Id FROM Tenants WHERE IsActive=1 AND OwnerId=@UserId";

            tenantId = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId });
        }
        return tenantId;
    }

    public async Task<IEnumerable<StoreInfo>> GetStoresByManagerId(int userId)
    {
        IEnumerable<StoreInfo> stores = Enumerable.Empty<StoreInfo>();

        using (DbConnection connection = new SqlConnection(_connectionString))
        {
            var query = @"SELECT Id, TenantId FROM Stores WHERE ManagerId=@UserId";

            stores = await connection.QueryAsync<StoreInfo>(query, new { UserId = userId });
        }

        return stores;
    }

    public async Task<IEnumerable<StoreInfo>> GetStoresByStaffId(int userId)
    {
        IEnumerable<StoreInfo> stores = Enumerable.Empty<StoreInfo>();

        using (DbConnection connection = new SqlConnection(_connectionString))
        {
            var query = @"SELECT S.Id, S.TenantId
                        FROM StoreStaffAssignments SSA
                        JOIN Stores S ON S.Id = SSA.StoreId
                        WHERE SSA.StaffId=@UserId";

            stores = await connection.QueryAsync<StoreInfo>(query, new { UserId = userId });
        }

        return stores;
    }

}
