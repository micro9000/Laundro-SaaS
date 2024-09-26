using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Data;

public interface IUserRepository
{
    Task<string?> GetUserRole(string email);
}

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string?> GetUserRole(string email)
    {
        string? userRole = null;
        var connectionString = _configuration.GetConnectionString("LaundroConnectionString");
        if (connectionString.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        using (DbConnection connection = new SqlConnection(connectionString))
        {
            var query = @"SELECT TOP 1 R.SystemKey
                                    FROM Users U
                                    JOIN Roles R ON R.Id=U.RoleId
                                    WHERE U.IsActive=1 AND R.IsActive=1 AND U.Email=@Email";

            userRole = await connection.ExecuteScalarAsync<string>(query, new
            {
                Email = email
            });
        }

        return userRole;
    }
}
