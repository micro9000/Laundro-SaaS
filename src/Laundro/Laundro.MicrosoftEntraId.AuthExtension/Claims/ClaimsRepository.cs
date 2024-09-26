using Dapper;
using Laundro.MicrosoftEntraId.AuthExtension.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface IClaimsRepository
{
    Task<UserInfo?> GetUserInfo(string email);
}

public class ClaimsRepository : IClaimsRepository
{
    private readonly IConfiguration _configuration;

    public ClaimsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<UserInfo?> GetUserInfo(string email)
    {
        UserInfo? userInfo = null;
        var connectionString = _configuration.GetConnectionString("LaundroConnectionString");
        if (connectionString.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        using (DbConnection connection = new SqlConnection(connectionString))
        {
            var query = @"SELECT TOP 1 U.Id As UserId,  R.SystemKey
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
}
